using System.Collections.Generic;
using System.Text;
using Infrastructure.DomainBase;
using NUnit.Framework;
using Tests.Utils.TestFixtures;
using Tests.Utils.Various;

namespace Tests.Unit.Infrastructure.DomainBase
{
    class ValidationExtenderTests : BaseTestFixture
    {
        [Test]
        public void CanValidateRequiredString()
        {
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => "".Required(), "Required");
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => " ".Required(), "Required");
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => "".Required("Value is required"), "Value is required");
            Assert.DoesNotThrow(() => "Non-empty string".Required());
        }

        [Test]
        public void CanValidateMinLengthString()
        {
            const string testString = "1234";

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => testString.MinLength(5), "Minimum length is 5. Value is '1234'.");
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => testString.MinLength(5, "Min length is {0}"), "Min length is 5");
            Assert.DoesNotThrow(() => testString.MinLength(4));
        }

        [Test]
        public void CanValidateMaxLengthString()
        {
            const string testString = "1234";

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => testString.MaxLength(3), "Maximum length is 3. Value is '1234'.");
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => testString.MaxLength(3, "Max length is {0}"), "Max length is 3");
            Assert.DoesNotThrow(() => testString.MaxLength(4));
        }

        [Test]
        public void CanValidateMinMaxLengthString()
        {
            const string testString = "1234";

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => testString.MinMaxLength(5, 8), "Value must be between or equal to 5 and 8. Value is '1234'.");
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => testString.MinMaxLength(5, 8, "Value must be between {0} and {1}"), "Value must be between 5 and 8");

            const string testString2 = "123456789";
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => testString2.MinMaxLength(5, 8), "Value must be between or equal to 5 and 8. Value is '123456789'.");
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => testString2.MinMaxLength(5, 8, "Value must be between {0} and {1}"), "Value must be between 5 and 8");

            const string testString3 = "123456";
            Assert.DoesNotThrow(() => testString3.MinMaxLength(5, 8));
        }

        [Test]
        public void CanValidateEmailaddress()
        {
            const string invalidEmailAddress = "robinwebpirates.nl";
            const string validEmailAddress = "robin@webpirates.nl";

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => invalidEmailAddress.ValidEmailAddress(), "'robinwebpirates.nl' is not a valid email address");
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => invalidEmailAddress.ValidEmailAddress("'{0}' is not valid"), "'robinwebpirates.nl' is not valid");
            Assert.DoesNotThrow(() => validEmailAddress.ValidEmailAddress());
        }

        [Test]
        public void CanValidateItemIsUniqueWithinCollection()
        {
            var ints = new List<int> { 1, 2, 3, 4, 5 };

            const int existingInt = 3;
            const int nonExistingInt = 6;

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => existingInt.Unique(ints, "Object with value {0} already exists in collection"), "Object with value 3 already exists in collection");
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => existingInt.Unique(ints), "3 already exists in the collection");
            Assert.DoesNotThrow(() => nonExistingInt.Unique(ints));
        }

        [Test]
        public void CanValidateRequiredObject()
        {
            StringBuilder emptyStringBuilder = null;
            var instanceOfStringBuilder = new StringBuilder();

            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => emptyStringBuilder.Required(), "StringBuilder is required");
            CustomAssert.ThrowsWithExceptionMessage<BusinessRuleViolationException>(() => emptyStringBuilder.Required("This value is required"), "This value is required");
            Assert.DoesNotThrow(() => instanceOfStringBuilder.Required());
        }
    }
}
