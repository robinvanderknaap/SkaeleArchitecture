using System;
using System.Web;

namespace Infrastructure.ExtensionMethods
{
    public static class AjaxRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers["X-Requested-With"] == "XMLHttpRequest"));
        }
    }
}
