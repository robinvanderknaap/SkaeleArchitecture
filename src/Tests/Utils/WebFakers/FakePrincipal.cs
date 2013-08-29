using System.Collections.Generic;
using System.Security.Principal;

namespace Tests.Utils.WebFakers
{
    public class FakePrincipal : IPrincipal
    {
        private readonly string _username;
        private List<string> _roles = new List<string>();

        public List<string> Roles
        {
            get { return _roles; }
        }
        
        public FakePrincipal() : this("TestUser")
        {            
        }

        public FakePrincipal(string username)
        {
            _username = username;
        }
        
        public IIdentity Identity
        {
            get { return new GenericIdentity(_username); }
        }

        public bool IsInRole(string role)
        {
            return _roles.Contains(role);
        }

        public void AddRoles(params string[] roles)
        {
            foreach (var role in roles)
            {
                _roles.Add(role);
            }
        }

        public void ClearRoles()
        {
            _roles = new List<string>();
        }
    }
}
