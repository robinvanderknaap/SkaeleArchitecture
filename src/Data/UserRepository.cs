using System;
using System.Linq;
using Domain.AbstractRepository;
using Domain.Users;
using Infrastructure.ApplicationSettings;
using Infrastructure.Encryption;
using Infrastructure.PasswordPolicies;
using NHibernate;
using NHibernate.Linq;

namespace Data
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly ISession _session;
        private readonly IPasswordPolicy _passwordPolicy;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IEncryptor _encryptor;

        public UserRepository(
            ISession session, 
            IPasswordPolicy passwordPolicy, 
            IApplicationSettings applicationSettings,
            IEncryptor encryptor
        )
        {
            _session = session;
            _passwordPolicy = passwordPolicy;
            _applicationSettings = applicationSettings;
            _encryptor = encryptor;
        }

        public override ISession Session
        {
            get { return _session; }
        }
        
        public bool Authenticate(string email, string password)
        {
            var maxAttempts = _applicationSettings.MaxLoginAttempts;
            var coolOffPeriod = TimeSpan.FromSeconds(_applicationSettings.CoolOffPeriod);

            // Retrieve hashed password from database
            const string query = "SELECT id, password, loginattempts, lastloginattempt FROM Users WHERE email = :Email and isactive = true";

            var result = Session.CreateSQLQuery(query)
                .SetString("Email", email)
                .UniqueResult<dynamic>();

            if (result == null)
                return false;

            var userId = (int)result[0];
            var hashedPassword = result[1].ToString();
            var loginAttempts = (int?)result[2];
            var lastloginattempt = (DateTime?)result[3];

            // If attempts equals maxAttempts or is bigger and last attempt less than cooloff period
            if (loginAttempts.HasValue && lastloginattempt.HasValue)
            {
                if (loginAttempts.Value >= maxAttempts && (DateTime.Now - lastloginattempt) < coolOffPeriod)
                {
                    // Locked out
                    return false;
                }
            }

            if (!DoesPasswordMatch(password, hashedPassword))
            {
                if (loginAttempts.HasValue)
                {
                    // Reset attempts when at maxAttempts, because user had to wait 15 minutes for this
                    if (loginAttempts.Value >= maxAttempts)
                        loginAttempts = 1;
                    else
                        loginAttempts++;
                }
                else
                {
                    loginAttempts = 1;
                }

                // Increment login attempts and set last login attempt
                const string updateQuery = "UPDATE users SET loginattempts = :loginAttempts, lastloginattempt = :lastLoginAttempt WHERE id = :userId";
                Session.CreateSQLQuery(updateQuery)
                    .SetInt32("loginAttempts", loginAttempts.Value)
                    .SetDateTime("lastLoginAttempt", DateTime.Now)
                    .SetInt32("userId", userId)
                    .UniqueResult();
                
                return false;
            }

            // Reset login attempts and last login attempt
            if (loginAttempts.HasValue || lastloginattempt.HasValue)
            {
                const string resetQuery = "UPDATE users SET loginattempts = null, lastloginattempt = null WHERE id = :userId";
                Session.CreateSQLQuery(resetQuery)
                    .SetInt32("userId", userId)
                    .UniqueResult();
            }
            return true;
        }

        public bool IsUserEmailUnique(string email)
        {
            return !Session.Query<User>().Any(x => x.Email == email);
        }

        public void Create(User user, string password)
        {
            if (user.Id != 0)
                throw new InvalidOperationException("Can't create an existing user");

            if (!_passwordPolicy.Validate(password))
                throw new ArgumentException("Password does not comply to password policy");

            base.Save(user);

            // Store password seperately, because password is not a member of the user entity
            const string query = "UPDATE Users SET Password = :Password WHERE Id = :UserId";

            Session.CreateSQLQuery(query)
                .SetInt32("UserId", user.Id)
                .SetString("Password", CreateHashedPassword(password))
                .UniqueResult();
        }

        public override void Save(User user)
        {
            if (user.IsTransient())
            {
                throw new InvalidOperationException("Cannot save user if not created first.");
            }

            base.Save(user);
        }

        public void ChangePassword(User user, string password)
        {
            if (user.Id == 0)
                throw new InvalidOperationException("Cannot change password for user who is not yet been persisted");

            if (!_passwordPolicy.Validate(password))
                throw new ArgumentException("Password does not comply to password policy");

            // Login attemps is set to null, this way users who are lockout can re-enter quickly by requesting a new password
            // Hash for new password request is set to null to disable the link for a new password

            const string query = "UPDATE Users SET Password = :Password, loginattempts = null, lastloginattempt = null, newpasswordrequested = null, newpasswordrequestedhash = null WHERE Id = :UserId";

            Session.CreateSQLQuery(query)
                .SetInt32("UserId", user.Id)
                .SetString("Password", CreateHashedPassword(password))
                .UniqueResult();
        }

        public string GetHashForNewPasswordRequest(User user)
        {
            // var hash = HttpUtility.UrlEncode(Encryptor.Sha256Encrypt(Guid.NewGuid().ToString()));
            var hash = _encryptor.Sha512Encrypt(Guid.NewGuid().ToString());

            // Create update query for password
            const string query = "UPDATE Users SET NewPasswordRequestedHash = :Hash, NewPasswordRequested = :HashDate WHERE Id = :UserId";

            // Execute query
            Session.CreateSQLQuery(query)
                .SetInt32("UserId", user.Id)
                .SetString("Hash", hash)
                .SetDateTime("HashDate", DateTime.Now)
                .UniqueResult();

            return hash;
        }

        public bool IsHashForNewPasswordRequestValid(User user, string hash)
        {
            // Create select query to retrieve hashed password
            const string query = "SELECT NewPasswordRequestedHash FROM Users WHERE id = :Id and isactive = true and NewPasswordRequested > :Created";

            // Execute query
            var hashInDb = Session.CreateSQLQuery(query)
                .SetInt32("Id", user.Id)
                .SetDateTime("Created", DateTime.Now.AddMinutes(-_applicationSettings.ExpirationPeriodNewPasswordrequest))
                .UniqueResult<string>();

            return hashInDb != null && hashInDb == hash;
        }

        private string CreateHashedPassword(string password)
        {
            // Create salt and pepper
            var random = new Random();
            var salt = String.Format("{0:X8}", random.Next(0x1000000));
            var pepper = String.Format("{0:X8}", random.Next(0x1000000));

            // Add salt and papper to password
            var passwordHash = salt + password + pepper;

            // Encrypt password together with salt and pepper
            passwordHash = _encryptor.Sha512Encrypt(passwordHash);

            // Add salt and pepper to the hashed password to be able to determine what the salt and pepper are
            passwordHash = salt + passwordHash + pepper;

            return passwordHash;
        }

        private bool DoesPasswordMatch(string password, string hashedPasswordFromDatabase)
        {
            var salt = hashedPasswordFromDatabase.Substring(0, 8);
            var pepper = hashedPasswordFromDatabase.Substring(hashedPasswordFromDatabase.Length - 8, 8);

            var hashedPassword = salt + _encryptor.Sha512Encrypt(salt + password + pepper) + pepper;

            return hashedPassword == hashedPasswordFromDatabase;
        }
    }
}
