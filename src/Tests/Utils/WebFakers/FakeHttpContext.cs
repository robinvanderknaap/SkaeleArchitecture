using System.Security.Principal;
using System.Web;

namespace Tests.Utils.WebFakers
{
    public class FakeHttpContext : HttpContextBase
    {
        private readonly IPrincipal _user;

        public FakeHttpContext()
            : this(new FakePrincipal())
        {
        }
        
        public FakeHttpContext(IPrincipal user)
        {
            _user = user;
        }

        public override IPrincipal User
        {
            get
            {
                return _user;
            }
        }
    }
}
