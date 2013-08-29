using System;
using System.Globalization;
using Domain.AbstractRepository;
using Domain.Users;
using Infrastructure.DomainBase;
using Moq;
using NUnit.Framework;
using Tests.Utils.TestFixtures;
using Tests.Utils.Various;

namespace Tests.Unit.Domain
{
    class UserTests : BaseTestFixture
    {
        [Test]
        public void CanCreateUser()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique:true);
            var culture = new CultureInfo("nl-NL");

            var user = new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings);

            Assert.AreEqual("robin@skaele.nl", user.Email);
            Assert.AreEqual("Robin van der Knaap", user.DisplayName);
            Assert.AreEqual(culture, user.Culture);
            Assert.AreEqual(0, user.Roles.Count);
        }

        [Test]
        public void CannotCreateInvalidUser()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique:true);
            var culture = new CultureInfo("nl-NL");

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => new User(" ", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings), 
                "Email address is required"
            );

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => new User("robinskaelenl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings),
                "'robinskaelenl' is not a valid email address"
            );

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => new User("robin@skaele.nl", " ", culture, userRepository, DefaultTestApplicationSettings),
                "Display name is required"
            );

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => new User("robin@skaele.nl", "Robin van der Knaap", null, userRepository, DefaultTestApplicationSettings),
                "Culture is required"
            );

            Assert.Throws<NullReferenceException>(
                () => new User("robin@skaele.nl", "Robin van der Knaap", culture, null, DefaultTestApplicationSettings)
            );

            Assert.Throws<NullReferenceException>(
                () => new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, null)
            );
        }

        [Test]
        public void CannotInvalidateUser()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique: true);
            var culture = new CultureInfo("nl-NL");

            var user = new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings);

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => user.SetEmail(" ", userRepository),
                "Email address is required"
            );

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => user.SetEmail("robinskaelenl", userRepository),
                "'robinskaelenl' is not a valid email address"
            );

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => user.SetCulture(null, DefaultTestApplicationSettings),
                "Culture is required"
            );

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => user.SetCulture(new CultureInfo("en-GB"), DefaultTestApplicationSettings),
                "Culture is not accepted"
            );

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => user.DisplayName = " ",
                "Display name is required"
            );
        }

        [Test]
        public void CanChangeEmail()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique: true);
            var culture = new CultureInfo("nl-NL");

            var user = new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings);

            user.SetEmail("daan@skaele.nl", userRepository);

            Assert.AreEqual("daan@skaele.nl", user.Email);
        }

        [Test]
        public void CannotCreateUserWithDuplicateEmail()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique: false);
            var culture = new CultureInfo("nl-NL");

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings),
                "Email must be unique"
            );
        }

        [Test]
        public void CannotChangeEmailToDuplicateEmail()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique: true);
            var culture = new CultureInfo("nl-NL");

            var user = new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings);

            var userRepositoryEmailNotUnique = GetFakeRepositoryInstance(emailIsUnique: false);

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(
                () => user.SetEmail("daan@skaele.nl", userRepositoryEmailNotUnique),
                "Email must be unique"
            );
        }

        [Test]
        public void CanChangeCulture()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique: true);
            var culture = new CultureInfo("nl-NL");

            var user = new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings);

            user.SetCulture(new CultureInfo("en-US"), DefaultTestApplicationSettings);

            Assert.AreEqual(new CultureInfo("en-US"), user.Culture);
        }

        [Test]
        public void CanAddRolesToUser()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique: true);
            var culture = new CultureInfo("nl-NL");

            var user = new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings);

            user.Roles.Add(Role.Administrator);
            user.Roles.Add(Role.User);

            Assert.AreEqual(2, user.Roles.Count);
            Assert.IsTrue(user.IsInRole(Role.Administrator));
            Assert.IsTrue(user.IsInRole("user"));
        }

        [Test]
        public void CannotAddDuplicateRolesToUser()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique: true);
            var culture = new CultureInfo("nl-NL");

            var user = new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings);

            user.Roles.Add(Role.Administrator);
            user.Roles.Add(Role.Administrator);

            Assert.AreEqual(1, user.Roles.Count);
        }

        [Test]
        public void CanDetermineIfUserIsInRole()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique: true);
            var culture = new CultureInfo("nl-NL");

            var user = new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings);

            user.Roles.Add(Role.Administrator);

            Assert.IsTrue(user.IsInRole(Role.Administrator));
            Assert.IsFalse(user.IsInRole("user"));
        }

        [Test]
        public void CanGetIdentity()
        {
            var userRepository = GetFakeRepositoryInstance(emailIsUnique: true);
            var culture = new CultureInfo("nl-NL");

            var user = new User("robin@skaele.nl", "Robin van der Knaap", culture, userRepository, DefaultTestApplicationSettings);

            Assert.AreEqual("robin@skaele.nl", user.Identity.Name);
            Assert.IsTrue(user.Identity.IsAuthenticated);
        }

        private static IUserRepository GetFakeRepositoryInstance(bool emailIsUnique)
        {
            var userRepository = new Mock<IUserRepository>();

            // Setup IsUserEmailUnique method, must return true or false as specified in parameter
            userRepository
                .Setup(x => x.IsUserEmailUnique("robin@skaele.nl"))
                .Returns(emailIsUnique);

            userRepository
                .Setup(x => x.IsUserEmailUnique("daan@skaele.nl"))
                .Returns(emailIsUnique);

            return userRepository.Object;
        }
    }
}
