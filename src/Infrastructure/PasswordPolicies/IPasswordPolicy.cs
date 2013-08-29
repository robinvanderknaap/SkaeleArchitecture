namespace Infrastructure.PasswordPolicies
{
    /// <summary>
    ///     Interface for password policy
    /// </summary>
    public interface IPasswordPolicy
    {
        /// <summary>
        ///     Validate password against policy
        /// </summary>
        /// <param name = "password">Password</param>
        /// <returns>Boolean indicating if password is valid according to policy</returns>
        bool Validate(string password);
    }
}