using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Data;
using Domain.AbstractRepository;
using Infrastructure.ApplicationSettings;
using Infrastructure.Cache;
using Infrastructure.Encryption;
using Infrastructure.Loggers;
using Infrastructure.PasswordPolicies;
using Infrastructure.Translations;
using MassTransit;
using NHibernate;
using Web.Cultures;

namespace Web.App_Start
{
    public class AutoFac
    {
        public static IContainer Container;
        
        public static void Setup()
        {
            var builder = new ContainerBuilder();
            
            // Register controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            
            // Register modules
            builder.RegisterModule(new ServiceBusModule());
            builder.RegisterModule(new NHibernateSessionModule());
            builder.RegisterModule(new TranslationServiceModule());
            builder.RegisterModule(new LoggerModule());
            
            // Register types
            builder.RegisterType<ApplicationSettings>().As<IApplicationSettings>().SingleInstance();
            builder.RegisterType<InMemoryKeyValueCache>().As<IKeyValueCache>().SingleInstance();
            builder.RegisterType<TranslationRepository>().As<ITranslationRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<LogRepository>().As<ILogRepository>();
            builder.RegisterType<DefaultEncryptor>().As<IEncryptor>();
            builder.RegisterType<CultureService>().As<ICultureService>();
            builder.Register(c => new RegularExpressionPasswordPolicy(c.Resolve<IApplicationSettings>().PasswordPolicy)).As<IPasswordPolicy>();

            Container = builder.Build();
            
            // Enables autofac to resolve controllers instead of default asp.net mvc implementation
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
        }
    }

    public class ServiceBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => ServiceBusFactory.New
                (
                    sbc =>
                    {
                        sbc.UseMsmq(x =>
                        {
                            x.VerifyMsmqConfiguration();
                            x.UseMulticastSubscriptionClient();
                        });
                        sbc.ReceiveFrom(c.Resolve<IApplicationSettings>().WebMessageQueueAddress);
                    }
                )
            )
            .As<IServiceBus>()
            .SingleInstance();
        }
    }

    public class NHibernateSessionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    var httpContextItems = HttpContext.Current.Items;

                    ISession session;
                    if (!httpContextItems.Contains(MvcApplication.SessionKey))
                    {
                        // Create an NHibernate session for this request
                        session = MvcApplication.SessionFactory.OpenSession();
                        httpContextItems.Add(MvcApplication.SessionKey, session);
                    }
                    else
                    {
                        // Re-use the NHibernate session for this request
                        session = (ISession)httpContextItems[MvcApplication.SessionKey];
                    }
                    return session;
                })
                .As<ISession>();
        }
    }

    public class TranslationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var translations = c.Resolve<ITranslationRepository>().GetAll();
                var applicationSettings = c.Resolve<IApplicationSettings>();

                var culture = applicationSettings.DefaultCulture;

                try
                {
                    culture = c.Resolve<ICultureService>().GetCulture();
                }
                catch (Exception exception)
                {
                    c.Resolve<ILogger>().Fatal("Failed to retrieve culture from user", exception);
                }

                if (!applicationSettings.AcceptedCultures.Contains(culture))
                {
                    culture = applicationSettings.DefaultCulture;
                }

                return new TranslationService(translations, culture);
            })
            .As<ITranslationService>();
        }
    }

    public class LoggerModule : Module
    {
        private Type _type;

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            _type = registration.Activator.LimitType;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var httpContext = HttpContext.Current;
                var className = _type.FullName;
                return httpContext != null ?
                    new NLogLogger(c.Resolve<IApplicationSettings>(), className, new HttpContextWrapper(httpContext)) :
                    new NLogLogger(c.Resolve<IApplicationSettings>(), className);
            })
            .As<ILogger>();
        }
    }
}