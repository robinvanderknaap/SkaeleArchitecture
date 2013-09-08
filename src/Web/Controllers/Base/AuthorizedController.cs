using System.Web.Mvc;

namespace Web.Controllers.Base
{
    [Authorize]
    public abstract class AuthorizedController : BaseController
    {
    }
}
