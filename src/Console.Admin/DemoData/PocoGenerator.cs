using System.Collections.Generic;
using System.Globalization;
using AutoPoco;
using AutoPoco.DataSources;
using AutoPoco.Engine;
using Data;
using Domain.Users;
using Infrastructure.ApplicationSettings;
using Infrastructure.Translations;

namespace Console.Admin.DemoData
{
    public class PocoGenerator
    {
        private static readonly IGenerationSession Session;

        static PocoGenerator()
        {
            Session = GetPocoFactory().CreateSession();
        }

        private static IGenerationSessionFactory GetPocoFactory()
        {
            // Return factory for poco's
            return AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c => c.UseDefaultConventions());
                x.AddFromAssemblyContainingType<PocoUser>();
            
                x.Include<PocoUser>()
                    .Setup(u => u.Email).Use<ExtendedEmailAddressSource>("skaele.nl")
                    .Setup(u => u.DisplayName).Use<FirstNameSource>()
                    .Setup(u => u.IsActive).Use<BooleanSource>();
            });
        }

        public static List<User> CreateUsers(int amount, UserRepository userRepository)
        {
            var users = new List<User>();
            
            foreach (var pocoUser in Session.List<PocoUser>(100).Get())
            {
                var user = new User(pocoUser.Email, pocoUser.DisplayName, new CultureInfo("nl-NL"), userRepository, new ApplicationSettings());
                userRepository.Create(user, "secret");
                users.Add(user);
            }

            return users;
        }

        public static List<User> CreateAdministrators(UserRepository userRepository)
        {
            var users = new List<User>();
            
            var robin = new User("robin@webpirates.nl", "Robin van der Knaap", new CultureInfo("nl-NL"), userRepository, new ApplicationSettings());
            userRepository.Create(robin, "secret");
            users.Add(robin);

            var daan = new User("daan@webpirates.nl", "Daan le Duc", new CultureInfo("nl-NL"), userRepository, new ApplicationSettings());
            userRepository.Create(daan, "secret");
            users.Add(daan);

            var johan = new User("jvdvleuten@gmail.com", "Johan van der Vleuten", new CultureInfo("nl-NL"), userRepository, new ApplicationSettings());
            userRepository.Create(johan, "secret");
            users.Add(johan);

            return users;
        }

        public static List<Translation> CreateTranslations(TranslationRepository translationsRepository)
        {
            var english = new CultureInfo("en-US");
            var dutch = new CultureInfo("nl-NL");

            var translations = new List<Translation>
            {
                // en-US
                new Translation("Login", "Login", english),
                new Translation("Logout", "Logout", english),
                new Translation("Home", "Home", english),
                new Translation("Users", "Users", english),
                new Translation("SendEmail", "Send email", english),
                new Translation("FlashMessages", "Flash messages", english),
                new Translation("Unauthorized", "Unauthorized", english),
                new Translation("Forbidden", "Forbidden", english),
                new Translation("PageNotFound", "Page not found", english),
                new Translation("InternalServerError", "Internal server error", english),
                new Translation("DearSirOrMadam", "Dear sir or madam", english),
                new Translation("KindRegards", "Kind regards", english),
                new Translation("ResetPassword", "Reset password", english),
                new Translation("ResetPasswordMailMessage", "We received a request to change your password. By clicking the link below you are able to reset your password.\r\nIn case you did not initiate this request, you can simply ignore this message, your password will remain unchanged.\r\n", english),
                new Translation("ResetPasswordMailMessageText", "We received a request to change your password. By using the link below you are able to reset your password.\r\nIn case you did not initiate this request, you can simply ignore this message, your password will remain unchanged.\r\n", english),
                new Translation("Required", "Required", english),
                new Translation("Email", "Email", english),
                new Translation("Password", "Password", english),
                new Translation("RepeatPassword", "Repeat password", english),
                new Translation("SignIn", "Sign in", english),
                new Translation("EmailAndPasswordCombinationNotValid", "Email and/or password is not valid", english),
                new Translation("InvalidEmailAddress", "Emailaddress is not valid", english),
                new Translation("UserWithEmailAddressWasNotFound", "User with emailaddress '{0}' was not found", english),
                new Translation("Send", "Send", english),
                new Translation("Back", "Back", english),
                new Translation("ChangePassword", "Change password", english),
                new Translation("PasswordsDoNotMatch", "Passwords do not match", english),
                new Translation("RequestToResetYourPassword", "Request to reset your password", english),
                new Translation("Success", "Success", english),
                new Translation("AnEmailWasSendToYourEmailaddressWithInstructionsOnHowToResetYourPassword", "An email was send to your emailaddress with instructions on how to reset your password", english),
                new Translation("YourPasswordWasChangedSuccessfully", "Your password was changed successfully", english),
                new Translation("ChuckNorrisFacts", "Chuck Norris Facts", english),
                new Translation("Shortcuts", "Shortcuts", english),
                new Translation("MorePages", "More pages", english),
                new Translation("PasswordShouldContainAtLeast5Characters", "Password should contain at least 5 characters", english),
                
                
                // nl-NL
                new Translation("Login", "Inloggen", dutch),
                new Translation("Logout", "Uitloggen", dutch),
                new Translation("Home", "Thuis", dutch),
                new Translation("Users", "Gebruikers", dutch),
                new Translation("SendEmail", "Verstuur e-mail", dutch),
                new Translation("FlashMessages", "Notificatie berichten", dutch),
                new Translation("Unauthorized", "Onbevoegd", dutch),
                new Translation("Forbidden", "Verboden", dutch),
                new Translation("PageNotFound", "Pagina niet gevonden", dutch),
                new Translation("InternalServerError", "Interne server fout", dutch),
                new Translation("DearSirOrMadam", "Geachte heer, mevrouw", dutch),
                new Translation("KindRegards", "Vriendelijke groeten", dutch),
                new Translation("ResetPassword", "Reset wachtwoord", dutch),
                new Translation("ResetPasswordMailMessage", "Een aanvraag voor wijziging van uw wachtwoord is bij ons aangekomen. Door op onderstaande link te klikken kunt u een nieuw wachtwoord opgeven.\r\nIndien u geen verzoek tot wijziging heeft gedaan kunt u deze email als niet verzonden beschouwen, uw bestaande wachtwoord blijft dan intact.\r\n", dutch),
                new Translation("ResetPasswordMailMessage", "Een aanvraag voor wijziging van uw wachtwoord is bij ons aangekomen. Door onderstaande link te gebruiken kunt u een nieuw wachtwoord opgeven.\r\nIndien u geen verzoek tot wijziging heeft gedaan kunt u deze email als niet verzonden beschouwen, uw bestaande wachtwoord blijft dan intact.\r\n", dutch),
                new Translation("Required", "Verplicht", dutch),
                new Translation("Email", "Email", dutch),
                new Translation("Password", "Wachtwoord", dutch),
                new Translation("RepeatPassword", "Herhaal wachtwoord", dutch),
                new Translation("SignIn", "Inloggen", dutch),
                new Translation("EmailAndPasswordCombinationNotValid", "Email en/of wachtwoord is niet geldig", dutch),
                new Translation("InvalidEmailAddress", "Emailadres is niet geldig", dutch),
                new Translation("UserWithEmailAddressWasNotFound", "Gebruiker met emailadres '{0}' is niet gevonden", dutch),
                new Translation("Send", "Verzenden", dutch),
                new Translation("Back", "Terug", dutch),
                new Translation("ChangePassword", "Wachtwoord wijzigen", dutch),
                new Translation("PasswordsDoNotMatch", "Wachtwoorden komen niet overeen", dutch),
                new Translation("RequestToResetYourPassword", "Verzoek om uw wachtwoord te wijzigen", dutch),
                new Translation("Success", "Succes", dutch),
                new Translation("AnEmailWasSendToYourEmailaddressWithInstructionsOnHowToResetYourPassword", "Een email is naar u toegestuurd met instructies hoe u uw wachtwoord kunt resetten.", dutch),
                new Translation("YourPasswordWasChangedSuccessfully", "Uw wachtwoord is succesvol aangepast", dutch),
                new Translation("ChuckNorrisFacts", "Chuck Norris Feiten", dutch),
                new Translation("Shortcuts", "Shortcuts", dutch),
                new Translation("MorePages", "Meer pagina's", dutch),
                new Translation("PasswordShouldContainAtLeast5Characters", "Wachtwoord dient minimaal 5 tekens te bevatten", dutch)
            };

            translations.ForEach(translationsRepository.Save);

            return translations;
        }
    }
}
