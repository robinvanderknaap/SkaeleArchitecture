using Infrastructure.DomainBase;

namespace Tests.Unit.Infrastructure.DomainBase.TestClasses
{
    /// <summary>
    /// This class is invalid because value objects should not contain domain signatures, this is only allowed for entities
    /// </summary>    
    public class InvalidTestValueObject : ValueObject
    {
        [DomainSignature]
        public string Firstname { get; set; }
        
        [DomainSignature]
        public string Lastname { get; set; }
    }
}
