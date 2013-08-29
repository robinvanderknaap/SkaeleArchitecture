using System;

namespace Infrastructure.DomainBase
{
    [Serializable]
    public class BusinessRuleViolationException : Exception
    {
        public BusinessRuleViolationException(string message) : base(message)
        {            
        }
    }
}
