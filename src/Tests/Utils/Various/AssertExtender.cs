using System;
using NUnit.Framework;

namespace Tests.Utils.Various
{
    public static class CustomAssert
    {
        public static void ThrowsWithExceptionMessage<T>(TestDelegate code, string message) where T : Exception
        {
            Assert.Throws<T>(code);

            try
            {
                code.Invoke();
            }
            catch (T exception)
            {
                Assert.AreEqual(message, exception.Message);
            }
        }
    }
}
