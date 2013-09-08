using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using Domain.Users;
using Infrastructure.ApplicationSettings;

namespace Web.Cultures
{
    public class CultureService : ICultureService
    {
        private readonly IApplicationSettings _applicationSettings;

        public CultureService(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public CultureInfo GetCulture()
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        public void SetCulture(HttpContextBase httpContext)
        {
            CultureInfo culture = null;

            var user = httpContext.User.Identity.IsAuthenticated ? (User)httpContext.User : null;
            var currentCulture = httpContext.Request.Cookies["CurrentCulture"];
            var languages = httpContext.Request.UserLanguages;

            if (user != null)
            {
                culture = user.Culture;
            }
            else if (currentCulture != null)
            {
                culture = new CultureInfo(currentCulture.Value);
            }
            else if (languages != null && languages.Length != 0)
            {
                culture = CultureInfo.CreateSpecificCulture(languages.First());
            }

            if (culture == null || !_applicationSettings.AcceptedCultures.Contains(culture))
            {
                culture = _applicationSettings.DefaultCulture;
            }

            SetCulture(culture, httpContext);
        }

        public void SetCulture(CultureInfo culture, HttpContextBase httpContext)
        {
            var currentCulture = httpContext.Request.Cookies["CurrentCulture"];

            if (currentCulture == null || currentCulture.Value != culture.ToString())
            {

                var cookie = new HttpCookie("CurrentCulture")
                    {
                        Value = culture.ToString(),
                        Expires = DateTime.Now.AddYears(30)
                    };

                httpContext.Response.Cookies.Add(cookie);
            }

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}