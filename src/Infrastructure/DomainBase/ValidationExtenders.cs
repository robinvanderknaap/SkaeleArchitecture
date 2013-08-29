using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Infrastructure.DomainBase
{
    public static class ValidationExtenders
    {
        public static string Required(this string value, string exceptionMessage = "Required")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessRuleViolationException(exceptionMessage);
            }
            return value;
        }
        
        public static string MaxLength(this string value, int maxLength, string exceptionMessage = "Maximum length is {0}. Value is '{1}'.")
        {
            if (value.Length > maxLength)
            {
                throw new BusinessRuleViolationException(string.Format(exceptionMessage, maxLength, value));
            }
            return value;
        }

        public static string MinLength(this string value, int minLength, string exceptionMessage = "Minimum length is {0}. Value is '{1}'.")
        {
            if (value.Length < minLength)
            {
                throw new BusinessRuleViolationException(string.Format(exceptionMessage, minLength, value));
            }
            return value;
        }

        public static string MinMaxLength(this string value, int minLength, int maxLength,
                                           string exceptionMessage = "Value must be between or equal to {0} and {1}. Value is '{2}'.")
        {
            int valueLength = value.Length;
            exceptionMessage = string.Format(exceptionMessage, minLength, maxLength, value);

            if (valueLength < minLength || valueLength > maxLength)
            {
                throw new BusinessRuleViolationException(exceptionMessage);
            }

            return value;
        }

        public static string ValidEmailAddress(this string value, string exceptionMessage = "'{0}' is not a valid email address")
        {
            var emailRegEx = new Regex("^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,6}$", RegexOptions.IgnoreCase);

            if (!emailRegEx.IsMatch(value))
            {
                throw new BusinessRuleViolationException(string.Format(exceptionMessage, value));
            }
            return value;
        }
        
        public static T Unique<T>(this T value, ICollection<T> collection, string exceptionMessage = "{0} already exists in the collection")
        {
            if (collection.Contains(value))
            {
                throw new BusinessRuleViolationException(string.Format(exceptionMessage, value));
            }
            return value;
        }
        
        public static T Required<T>(this T value, string exceptionMessage = "") where T : class
        {
            if (value == null)
            {
                string message = string.IsNullOrWhiteSpace(exceptionMessage)
                                     ? typeof(T).Name + " is required"
                                     : exceptionMessage;
                throw new BusinessRuleViolationException(message);
            }
            return value;
        }
    }
}
