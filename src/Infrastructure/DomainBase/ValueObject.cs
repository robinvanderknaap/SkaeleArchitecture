using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrastructure.DomainBase
{
    [Serializable]
    public abstract class ValueObject : BaseObject
    {
        /// <summary>
        /// The getter for SignatureProperties for value objects should include the properties 
        /// which make up the entirety of the object's properties; that's part of the definition 
        /// of a value object.
        /// </summary>
        /// <remarks>
        /// This ensures that the value object has no properties decorated with the 
        /// [DomainSignature] attribute.
        /// </remarks>
        protected override IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties()
        {
            var invalidlyDecoratedProperties = GetType().GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(DomainSignatureAttribute), true));

            if (invalidlyDecoratedProperties.Any())
                throw new InvalidOperationException("Properties were found within " + GetType() + @" having the
                [DomainSignature] attribute. The domain signature of a value object includes all
                of the properties of the object by convention; consequently, adding [DomainSignature]
                to the properties of a value object's properties is misleading and should be removed. 
                Alternatively, you can inherit from Entity if that fits your needs better.");

            return GetType().GetProperties();
        }

        public static bool operator ==(ValueObject valueObject1, ValueObject valueObject2)
        {
            if ((object)valueObject1 == null)
                return (object)valueObject2 == null;

            return valueObject1.Equals(valueObject2);
        }

        public static bool operator !=(ValueObject valueObject1, ValueObject valueObject2)
        {
            return !(valueObject1 == valueObject2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
