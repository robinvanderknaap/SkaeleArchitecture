using System;
using System.Linq;
using System.Text;
using Console.Admin.DemoData;
using Data;
using Data.Utils;
using Infrastructure.ApplicationSettings;
using Infrastructure.Cache;
using Infrastructure.Encryption;
using Infrastructure.Loggers;
using Infrastructure.PasswordPolicies;

namespace Console.Admin
{
    class Program
    {
        static void Main(params string[] args)
        {
            //NHibernateProfiler.Initialize();
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (args.Any())
            {
                ExecuteOption(args[0]);
            }
            else
            {
                string option;
                do
                {
                    // Create menu
                    var menu = new StringBuilder();
                    menu.AppendLine("(1) Create database schema");
                    menu.AppendLine("(2) Create database schema and demo data");
                    menu.AppendLine("(3) Log test");
                    menu.AppendLine("(x) Exit");

                    // Display menu and wait for user input
                    System.Console.WriteLine(menu);
                    option = System.Console.ReadKey().KeyChar.ToString();
                    System.Console.WriteLine();
                    ExecuteOption(option);
                    System.Console.WriteLine();
                } while (option.ToLower() != "x");
            }
        }

        private static void ExecuteOption(string option)
        {
            switch (option.ToLower())
            {
                case "1":
                    CreateDatabaseSchema();
                    break;
                case "2":
                    CreateDatabaseSchemaAndDemoData();
                    break;
                case "3":
                    LogTest();
                    break;
                case "x":
                    break;
                default:
                    System.Console.WriteLine("That's not an option!");
                    break;
            }
        }

        private static void CreateDatabaseSchema()
        {
            NHibernateHelper.CreateDatabaseSchema();
            System.Console.WriteLine("Database schema created");
        }

        private static void CreateDatabaseSchemaAndDemoData()
        {
            CreateDatabaseSchema();

            var session = NHibernateHelper.SessionFactory.OpenSession();
            var passwordPolicy = new RegularExpressionPasswordPolicy(".{5,}$");
            var translationsRepository = new TranslationRepository(session, new InMemoryKeyValueCache());
            var applicationSettings = new ApplicationSettings();
            var encryptor = new DefaultEncryptor();

            var userRepository = new UserRepository(session, passwordPolicy, applicationSettings, encryptor);

            // Create administrators
            PocoGenerator.CreateAdministrators(userRepository);

            // Create users
            PocoGenerator.CreateUsers(100, userRepository);

            session.Transaction.Begin();

            // Create translations
            PocoGenerator.CreateTranslations(translationsRepository);

            session.Transaction.Commit();

            // Create logitems
            PocoGenerator.CreateLogItems(new NLogLogger(applicationSettings, "Console.Admin"));
        }

        private static void LogTest()
        {
            var logger = new NLogLogger(new ApplicationSettings());
            logger.Debug("Just testing debug", "Debug");
            logger.Info("Just testing info", "Info");
            logger.Warn("Just testing warning", "Warning");
            logger.Error("Just testing error", "Error");
            logger.Fatal("Testing with exception", new Exception("TestException"), "Some details again");
        }
    }
}
