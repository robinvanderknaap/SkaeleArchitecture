using System;
using System.Linq;
using Infrastructure.Encryption;
using NUnit.Framework;
using Tests.Utils.TestFixtures;

namespace Tests.Unit.Infrastructure.Encryption
{
    class DefaultEncryptorTests : BaseTestFixture
    {
        [Test]
        public void CanCreateMd5EncryptedString()
        {
            const string testString = "String to be encrypted";
            var encryptedString = new DefaultEncryptor().Md5Encrypt(testString);

            Assert.AreEqual(32, encryptedString.Count());
            Assert.AreEqual("52a3800d865b9017ac36e4d036133a91", encryptedString);
        }

        [Test]
        public void CanCreateSha512EncryptedString()
        {
            const string testString = "String to be encrypted";
            var encryptedString = new DefaultEncryptor().Sha512Encrypt(testString);

            Assert.AreEqual(128, encryptedString.Count());
            Assert.AreEqual("d8022b70792f76ed94c007962dc6fcbcb15463ab79eedbaaa0c42293918252e9c73c6ea121a19238adcae68f24c71e971346b806e5ead70a61caaa3a13a8b62a", encryptedString);
        }

        [Test]
        public void ToBeMd5EncryptedPhraseCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultEncryptor().Md5Encrypt(null));
        }

        [Test]
        public void ToBeSha512EncryptedPhraseCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultEncryptor().Sha512Encrypt(null));
        }
    }
}
