using Infrastructure.DomainBase;

namespace Tests.Unit.Infrastructure.DomainBase.TestClasses
{
    public class TestEntity : Entity
    {
        public TestEntity(int id)
        {
            Id = id;
        }
        
        [DomainSignature]
        public string Firstname { get; set; }
        
        [DomainSignature]
        public string Lastname { get; set; }
    }
}
