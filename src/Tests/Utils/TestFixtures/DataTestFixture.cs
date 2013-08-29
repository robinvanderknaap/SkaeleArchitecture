using Data.Utils;
using Infrastructure.Encryption;
using Infrastructure.PasswordPolicies;
using NHibernate;
using NHibernate.Context;
using NUnit.Framework;

namespace Tests.Utils.TestFixtures
{
    public abstract class DataTestFixture : BaseTestFixture
    {
        protected ISession Session;
        protected ISessionFactory SessionFactory;
        protected IPasswordPolicy PasswordPolicy = new RegularExpressionPasswordPolicy(".{5,}$");
        
        protected IEncryptor Encryptor
        {
            get { return new DefaultEncryptor();}
        }

        [SetUp]
        public void Setup()
        {
            NHibernateHelper.CreateDatabaseSchema();
            SessionFactory = NHibernateHelper.SessionFactory;
            Session = NHibernateHelper.SessionFactory.OpenSession();
            CurrentSessionContext.Bind(Session);
        }

        [TearDown]
        public void TearDown()
        {
            var session = CurrentSessionContext.Unbind(SessionFactory);
            session.Close();
            Session.Dispose();
        }
    }
}
