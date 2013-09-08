using System;
using System.Reflection;
using System.ServiceProcess;
using Autofac;
using Data;
using Data.Utils;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Infrastructure.ApplicationSettings;
using Infrastructure.Cache;
using Infrastructure.Loggers;
using Infrastructure.Mailers;
using Infrastructure.TemplateMailMessages;
using Infrastructure.Translations;
using Magnum.Extensions;
using MassTransit;
using NHibernate;
using RazorMailMessage;
using RazorMailMessage.TemplateCache;
using RazorMailMessage.TemplateResolvers;

namespace WindowsService.MailSender
{
    public partial class MailSenderService : ServiceBase
    {
        private readonly IContainer _container;
        private IServiceBus _bus;

        public MailSenderService()
        {
            InitializeComponent();

            NHibernateProfiler.Initialize();

            _container = BuildContainer();
        }

        protected override void OnStart(string[] args)
        {
            _bus = _container.Resolve<IServiceBus>();
        }

        protected override void OnStop()
        {
            _bus.Dispose();
        }

        private IContainer BuildContainer()
        {
            // Use 'InstancePerLifetimeScope' to create instance which will be shared during the consuming of one message

            var builder = new ContainerBuilder();

            builder.RegisterModule(new ServiceBusModule());
            builder.RegisterModule(new TranslationServiceModule());
            builder.RegisterModule(new MailerModule());

            // Register razor mail message factory (have not found a way to put this in a module, due to the dependencyResolver parameter)
            builder.RegisterType<RazorMailMessageFactory>()
                .WithParameter(new NamedParameter("templateResolver", new DefaultTemplateResolver("Infrastructure", "TemplateMailMessages")))
                .WithParameter(new NamedParameter("templateBase", typeof(ViewBaseClass<>)))
                .WithParameter(new NamedParameter("dependencyResolver", new Func<Type, object>(x => _container.Resolve(x))))
                .WithParameter(new NamedParameter("templateCache", new InMemoryTemplateCache()))
                .As<IRazorMailMessageFactory>()
                .InstancePerLifetimeScope();

            builder.Register(c => NHibernateHelper.SessionFactory).SingleInstance();
            builder.Register(c => c.Resolve<ISessionFactory>().OpenSession()).As<ISession>().InstancePerLifetimeScope();
            builder.RegisterType<InMemoryKeyValueCache>().As<IKeyValueCache>().InstancePerLifetimeScope();
            builder.RegisterType<TranslationRepository>().As<ITranslationRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationSettings>().As<IApplicationSettings>().SingleInstance();
            builder.RegisterType<NLogLogger>().As<ILogger>().InstancePerLifetimeScope();
            
            return builder.Build();
        }

        public class ServiceBusModule : Autofac.Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                // Register all consumers in this assembly
                builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                    .Where(t => t.Implements<IConsumer>())
                    .AsSelf();

                builder.Register(c => ServiceBusFactory.New
                (
                    sbc =>
                    {
                        sbc.UseMsmq(x =>
                        {
                            x.VerifyMsmqConfiguration();
                            x.UseMulticastSubscriptionClient();
                        });
                        sbc.ReceiveFrom(c.Resolve<IApplicationSettings>().MailSenderMessageQueueAddress);


                        // This will find all of the consumers in the container and register them with the bus.
                        // Resolve<ILifeTimeScope> is used to get all registrations from the container, the loadfrom method filters out all IConsumers and ISaga's
                        // Loadfrom is an extension method for autofac implemented in masstransit (Separate package: MassTransit.Autofac)
                        sbc.Subscribe(x => x.LoadFrom(c.Resolve<ILifetimeScope>()));
                    }
                )
            )
            .As<IServiceBus>()
            .SingleInstance();
            }
        }

        public class TranslationServiceModule : Autofac.Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.Register(c =>
                    {
                        var translations = c.Resolve<ITranslationRepository>().GetAll();
                        return new TranslationService(translations, new ApplicationSettings().DefaultCulture);
                    }
                )
                .As<ITranslationService>()
                .InstancePerLifetimeScope();
            }
        }

        public class MailerModule : Autofac.Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.Register(c =>
                {
                    var applicationSettings = c.Resolve<IApplicationSettings>();
                    return new SmtpMailer
                    (
                        applicationSettings.SmtpHost,
                        applicationSettings.SmtpPort,
                        applicationSettings.SmtpUsername,
                        applicationSettings.SmtpPassword,
                        applicationSettings.SmtpSslEnabled,
                        c.Resolve<ILogger>()
                    );
                }
                )
                .As<IMailer>()
                .InstancePerLifetimeScope();
            }
        }
    }
}
