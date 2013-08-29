using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Data;
using Domain.AbstractRepository;
using Domain.Users;
using Infrastructure.ApplicationSettings;
using NUnit.Framework;
using Tests.Utils.ApplicationSettings;
using Tests.Utils.TestFixtures;
using Tests.Utils.Various;

namespace Tests.Integration.Data
{
    [TestFixture]
    class UserRepositoryTests : DataTestFixture
    {
        [Test]
        public void CanCreateUser()
        {
            var user = CreateUser();
            
            var userFromDb = GetUserRepository().GetOne(x => x.Email == "robin@skaele.nl");

            Assert.AreEqual(user, userFromDb);
            Assert.AreEqual(user.Email, userFromDb.Email);
            Assert.AreEqual(user.DisplayName, userFromDb.DisplayName);
            Assert.AreEqual(user.Culture, userFromDb.Culture);
            Assert.AreEqual(0, userFromDb.Roles.Count);
        }

        [Test]
        public void CanSaveUser()
        {
            var userRepository = GetUserRepository();
            CreateUser();

            var userFromDbBeforeSave = userRepository.GetOne(x => x.Email == "robin@skaele.nl");
            userFromDbBeforeSave.DisplayName = "Daan le Duc";

            userRepository.Save(userFromDbBeforeSave);

            Session.Flush();
            Session.Evict(userFromDbBeforeSave);

            var userFromDbAfterSave = userRepository.GetOne(x => x.Email == "robin@skaele.nl");

            Assert.AreEqual("Daan le Duc", userFromDbAfterSave.DisplayName);
        }

        [Test]
        public void CanAuthenticateUser()
        {
            var testApplicationSettings = new TestApplicationSettings
            {
                MaxLoginAttempts = 3
            };

            var userRepository = GetUserRepository(testApplicationSettings);

            CreateUser(testApplicationSettings);

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));
            Assert.IsFalse(userRepository.Authenticate("robin@skaele.n", "secret"));
            Assert.IsFalse(userRepository.Authenticate("robin@skaele.nl", "secre"));
        }

        [Test]
        public void UserCannotLoginAfterExceedingMaxLoginAttempts()
        {
            const int maxLoginAttempts = 3;

            var testApplicationSettings = new TestApplicationSettings 
            {
                MaxLoginAttempts = maxLoginAttempts
            };

            var userRepository = GetUserRepository(testApplicationSettings);

            CreateUser(testApplicationSettings);

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));

            for (var ctr = 0; ctr < maxLoginAttempts; ctr++)
            {
                userRepository.Authenticate("robin@skaele.nl", "wrongPassword");
            }

            Assert.IsFalse(userRepository.Authenticate("robin@skaele.nl", "secret"));
        }

        [Test]
        public void UserCanLoginWhenNotExceedingMaxLoginAttempts()
        {
            const int maxLoginAttempts = 5;

            var testApplicationSettings = new TestApplicationSettings
            {
                MaxLoginAttempts = maxLoginAttempts
            };

            var userRepository = GetUserRepository(testApplicationSettings);

            CreateUser(testApplicationSettings);

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));

            for (var ctr = 0; ctr < maxLoginAttempts - 1; ctr++)
            {
                userRepository.Authenticate("robin@skaele.nl", "wrongPassword");
            }

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));
        }

        [Test]
        public void UserCanLoginAfterCoolOffPeriodExpires()
        {
            const int maxLoginAttempts = 3;
            const int coolOffPeriod = 3; // seconds

            var testApplicationSettings = new TestApplicationSettings
            {
                MaxLoginAttempts = maxLoginAttempts,
                CoolOffPeriod = coolOffPeriod
            };

            var userRepository = GetUserRepository(testApplicationSettings);

            CreateUser(testApplicationSettings);

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));

            for (var ctr = 0; ctr < maxLoginAttempts; ctr++)
            {
                userRepository.Authenticate("robin@skaele.nl", "wrongPassword");
            }

            Assert.IsFalse(userRepository.Authenticate("robin@skaele.nl", "secret"));

            // Cool off
            Thread.Sleep(coolOffPeriod * 1000);

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));
        }

        [Test]
        public void UserLoginAttemptsAreResetAfterCoolOffPeriodExpires()
        {
            const int maxLoginAttempts = 5;
            const int coolOffPeriod = 2; // seconds

            var testApplicationSettings = new TestApplicationSettings
            {
                MaxLoginAttempts = maxLoginAttempts,
                CoolOffPeriod = coolOffPeriod
            };

            var userRepository = GetUserRepository(testApplicationSettings);

            CreateUser(testApplicationSettings);

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));

            for (var ctr = 0; ctr < maxLoginAttempts; ctr++)
            {
                userRepository.Authenticate("robin@skaele.nl", "wrongPassword");
            }

            Assert.IsFalse(userRepository.Authenticate("robin@skaele.nl", "secret"));

            // Cool off
            Thread.Sleep(coolOffPeriod * 1000);

            for (var ctr = 0; ctr < maxLoginAttempts - 1; ctr++)
            {
                userRepository.Authenticate("robin@skaele.nl", "wrongPassword");
            }

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));
        }

        [Test]
        public void UserLoginAttemptsAreResetAfterSuccessfullLogin()
        {
            const int maxLoginAttempts = 5;

            var testApplicationSettings = new TestApplicationSettings
            {
                MaxLoginAttempts = maxLoginAttempts
            };

            var userRepository = GetUserRepository(testApplicationSettings);

            CreateUser(testApplicationSettings);

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));

            for (var ctr = 0; ctr < maxLoginAttempts -1; ctr++)
            {
                userRepository.Authenticate("robin@skaele.nl", "wrongPassword");
            }

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));

            for (var ctr = 0; ctr < maxLoginAttempts - 1; ctr++)
            {
                userRepository.Authenticate("robin@skaele.nl", "wrongPassword");
            }

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));
        }

        [Test]
        public void CannotCreateUserWithInvalidPassword()
        {
            var userRepository = GetUserRepository();

            var user = new User("robin@skaele.nl", "Robin van der Knaap", new CultureInfo("nl-NL"), userRepository, DefaultTestApplicationSettings);

            CustomAssert.ThrowsWithExceptionMessage<ArgumentException>(
                () => userRepository.Create(user, "1234"),
                "Password does not comply to password policy"
            );
        }

        [Test]
        public void CannotCreateUserWhichIsAlreadyCreated()
        {
            var userRepository = GetUserRepository();

            var user = CreateUser();

            CustomAssert.ThrowsWithExceptionMessage<InvalidOperationException>(
                () => userRepository.Create(user, "secret"),
                "Can't create an existing user"
            );
        }

        [Test]
        public void CannotSaveUserWhichIsNotCreatedYet()
        {
            var userRepository = GetUserRepository();

            var user = new User("robin@skaele.nl", "Robin van der Knaap", new CultureInfo("nl-NL"), userRepository, DefaultTestApplicationSettings);

            CustomAssert.ThrowsWithExceptionMessage<InvalidOperationException>(
                () => userRepository.Save(user),
                "Cannot save user if not created first."
            );
        }

        [Test]
        public void CanChangePassword()
        {
            var userRepository = GetUserRepository();

            var user = CreateUser();

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));

            userRepository.ChangePassword(user, "newPassword");

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "newPassword"));
        }

        [Test]
        public void CannotChangePasswordForUserThatHasNotBeenCreatedYet()
        {
            var userRepository = GetUserRepository();

            var user = new User("robin@skaele.nl", "Robin van der Knaap", new CultureInfo("nl-NL"), userRepository, DefaultTestApplicationSettings);

            CustomAssert.ThrowsWithExceptionMessage<InvalidOperationException>(
                () => userRepository.ChangePassword(user, "newPassword"),
                "Cannot change password for user who is not yet been persisted"
            );
        }

        [Test]
        public void CannotChangePasswordWhenPasswordIsNotValid()
        {
            var userRepository = GetUserRepository();

            var user = CreateUser();

            CustomAssert.ThrowsWithExceptionMessage<ArgumentException>(
                () => userRepository.ChangePassword(user, "1234"),
                "Password does not comply to password policy"
            );
        }

        [Test]
        public void CanChangePasswordWhenMaxAttemptsIsExceeded()
        {
            const int maxLoginAttempts = 3;

            var testApplicationSettings = new TestApplicationSettings
            {
                MaxLoginAttempts = maxLoginAttempts,
                CoolOffPeriod = 1000
            };

            var userRepository = GetUserRepository(testApplicationSettings);

            var user = CreateUser(testApplicationSettings);

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "secret"));

            for (var ctr = 0; ctr < maxLoginAttempts; ctr++)
            {
                userRepository.Authenticate("robin@skaele.nl", "wrongPassword");
            }

            Assert.IsFalse(userRepository.Authenticate("robin@skaele.nl", "secret"));

            userRepository.ChangePassword(user, "newPassword");

            Assert.IsTrue(userRepository.Authenticate("robin@skaele.nl", "newPassword"));
        }

        [Test]
        public void CanGetHashForNewPassword()
        {
            var userRepository = GetUserRepository();
            var user = CreateUser();

            var hash = userRepository.GetHashForNewPasswordRequest(user);

            Assert.IsTrue(userRepository.IsHashForNewPasswordRequestValid(user, hash));
        }

        [Test]
        public void CanUseBaseRepositoryQueryMethodsOnUserEntity()
        {
            var userRepository = GetUserRepository();

            var user1 = new User("robin@skaele.nl", "Robin van der Knaap", new CultureInfo("nl-NL"), userRepository, DefaultTestApplicationSettings);
            var user2 = new User("daan@skaele.nl", "Daan le Duc", new CultureInfo("nl-NL"), userRepository, DefaultTestApplicationSettings);
            var user3 = new User("eric@skaele.nl", "Eric Smalley", new CultureInfo("nl-NL"), userRepository, DefaultTestApplicationSettings);

            userRepository.Create(user1, "secret");
            userRepository.Create(user2, "secret");
            userRepository.Create(user3, "secret");

            Session.Flush();
            Session.Evict(user1);
            Session.Evict(user2);
            Session.Evict(user3);

            Assert.AreEqual(user1, userRepository.Get(user1.Id));
            Assert.AreEqual(user2, userRepository.GetOne(x => x.Email == "daan@skaele.nl"));
            Assert.AreEqual(3, userRepository.GetAll().Count());
            Assert.AreEqual(2, userRepository.GetAll(x => x.DisplayName.Contains("c")).Count());
        }

        private IUserRepository GetUserRepository()
        {
            return GetUserRepository(new TestApplicationSettings());
        }

        private IUserRepository GetUserRepository(IApplicationSettings applicationSettings)
        {
            return new UserRepository(Session, PasswordPolicy, applicationSettings, Encryptor);
        }

        private User CreateUser()
        {
            return CreateUser(new TestApplicationSettings());
        }

        private User CreateUser(IApplicationSettings applicationSettings)
        {
            var userRepository = GetUserRepository(applicationSettings);

            var user = new User("robin@skaele.nl", "Robin van der Knaap", new CultureInfo("nl-NL"), userRepository, DefaultTestApplicationSettings);

            userRepository.Create(user, "secret");

            // Make sure user is evicted from session so it won't be cached
            Session.Flush();
            Session.Evict(user);

            return user;
        }
    }
}
