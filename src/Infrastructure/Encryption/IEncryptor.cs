namespace Infrastructure.Encryption
{
    public interface IEncryptor
    {
        string Md5Encrypt(string phrase);
        string Sha512Encrypt(string phrase);
    }
}