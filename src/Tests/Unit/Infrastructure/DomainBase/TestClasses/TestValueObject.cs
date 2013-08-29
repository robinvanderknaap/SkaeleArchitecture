using Infrastructure.DomainBase;

namespace Tests.Unit.Infrastructure.DomainBase.TestClasses
{
    public class TestValueObject : ValueObject
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
