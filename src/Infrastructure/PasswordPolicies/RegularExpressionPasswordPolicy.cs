using System.Text.RegularExpressions;

namespace Infrastructure.PasswordPolicies
{
    /// <summary>
    ///     Password policy based on regular expression
    /// </summary>
    public class RegularExpressionPasswordPolicy : IPasswordPolicy
    {
        private readonly string _regularExpression;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name = "regularExpression">Regular expression to validate the password</param>
        public RegularExpressionPasswordPolicy(string regularExpression)
        {
            _regularExpression = regularExpression;
        }

        #region IPasswordPolicy Members

        /// <summary>
        ///     Validate password
        /// </summary>
        /// <param name = "password">Password</param>
        /// <returns>Boolean indicating if password is valid according to policy</returns>
        public bool Validate(string password)
        {
            // Validate password using regex
            return new Regex(_regularExpression).IsMatch(password);
        }

        #endregion
    }
}