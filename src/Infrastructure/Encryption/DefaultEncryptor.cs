using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Encryption
{
    /// <summary>
    /// Collection of encryption methods
    /// </summary>
    public  class DefaultEncryptor : IEncryptor
    {
        public string Md5Encrypt(string phrase)
        {
            var bytes = Encoding.UTF8.GetBytes(phrase);
            bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
            var hashBuilder = new StringBuilder();

            foreach (byte b in bytes)
            {
                hashBuilder.Append(b.ToString("x2").ToLower());
            }

            return hashBuilder.ToString();
        }
        
        public string Sha512Encrypt(string phrase)
        {
            var bytes = Encoding.UTF8.GetBytes(phrase);
            bytes = new SHA512CryptoServiceProvider().ComputeHash(bytes);
            var hashBuilder = new StringBuilder();

            foreach (byte b in bytes)
            {
                hashBuilder.Append(b.ToString("x2").ToLower());
            }

            return hashBuilder.ToString();
        }        
    }
}