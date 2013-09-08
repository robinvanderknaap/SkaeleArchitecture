using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace Infrastructure.HtmlHelpers
{
    public static class MainMenuHelper
    {
        public static MainMenu MainMenu(this HtmlHelper helper)
        {
            return new MainMenu(helper.ViewContext.HttpContext.User, helper.ViewContext);
        }
    }

    public class MainMenu : IHtmlString
    {
        private readonly IList<MainMenuItem> _mainMenuItems = new List<MainMenuItem>();
        private readonly IPrincipal _user;
        private readonly ViewContext _viewContext;

        public MainMenu(IPrincipal user, ViewContext viewContext)
        {
            _user = user;
            _viewContext = viewContext;
        }
        
        public MainMenu Add(MainMenuItem mainMenuItem)
        {
            _mainMenuItems.Add(mainMenuItem);
            return this;
        }        
        
        public string ToHtmlString()
        {
            const string template = @"
                <ul class=""nav navbar-nav"">{0}
                </ul>
            ";

            return string.Format(template, 
                string.Join("", _mainMenuItems
                    .Select(x => x.ToString(_user, _viewContext))
                    .ToArray()));
        }
    }

    public class MainMenuItem
    {
        private string _text = string.Empty;
        private string _icon = string.Empty;
        private string _url = string.Empty;
        private string _controller = string.Empty;
        private string _action = string.Empty;
        private readonly IList<ISubMenuItem> _subMenuItems = new List<ISubMenuItem>();
        private readonly HashSet<string> _roles = new HashSet<string>();
        
        public MainMenuItem Text(string text)
        {
            _text = text.Trim();
            return this;
        }

        public MainMenuItem Icon(string icon)
        {
            _icon = icon.Trim();
            return this;
        }

        public MainMenuItem Url(string url)
        {
            _url = url.Trim();
            return this;
        }

        public MainMenuItem Action<T>(string action) where T : IController
        {
            _action = action;
            _controller = typeof(T).Name.Replace("Controller", "");
            return this;
        }

        public MainMenuItem Add(ISubMenuItem subMenuItem)
        {
            _subMenuItems.Add(subMenuItem);
            return this;
        }

        public MainMenuItem Roles(params string[] roles)
        {
            foreach (var role in roles)
            {
                _roles.Add(role);
            }
            return this;
        }

        public string ToString(IPrincipal user, ViewContext viewContext)
        {
            // If roles are specified skip item if is user is not in one of the roles needed for this item
            // This will make sure the user won't see links which aren't available to him
            if (_roles.Any() && !_roles.Any(user.IsInRole))
            {
                return string.Empty;
            }

            // Menu item contains sub items
            if (_subMenuItems.Any())
            {
                // Remove sub menu items which are not allowed to the user (we do this here, so we can remove redundant dividers after this)
                var notAllowedSubMenuItems = _subMenuItems.OfType<SubMenuItem>()
                    .Where(x => x._roles.Any() && !x._roles.Any(user.IsInRole))
                    .ToList();

                foreach (var notAllowedSubMenuItem in notAllowedSubMenuItems)
                {
                    _subMenuItems.Remove(notAllowedSubMenuItem);
                }                
                
                // Remove dividers at the end of the list
                while (_subMenuItems.Last().GetType() == typeof(Divider))
                {
                    _subMenuItems.Remove(_subMenuItems.Last());
                }
                
                const string template = @"
                    <li class=""dropdown"">
                        <a href=""javascript:;"" class=""dropdown-toggle"" data-toggle=""dropdown"">
                            <i class=""{0}""></i>
                            <span>{1}</span>
                            <b class=""caret""></b>
                        </a>
                        <ul class=""dropdown-menu"">
                            {2}
                        </ul>
                    </li>";

                return string.Format(template,
                    _icon,
                    _text,
                    string.Join("\r\n", _subMenuItems.Select(x => x.ToString(viewContext)).ToArray()
                    )
                ); 
            }
            
            // Menu item contains no subitems
            else
            {
                const string template = @"
                    <li{0}>
                        <a href=""{1}"">
                            <i class=""{2}""></i>
                            <span>{3}</span>
                        </a>
                    </li>";

                return string.Format(template,
                    IsActive(viewContext) ? " class=\"active\"" : "",
                    GetHyperLink(viewContext),
                    _icon,
                    _text
                );
            }
        }

        private bool IsActive(ViewContext viewContext)
        {
            if ((string)viewContext.RouteData.Values["action"] == _action && (string)viewContext.RouteData.Values["controller"] == _controller)
            {
                return true;
            }
            return false;
        }

        private string GetHyperLink(ViewContext viewContext)
        {
            // Make sure either url is set or action, not both.
            if (!string.IsNullOrWhiteSpace(_action) && !string.IsNullOrWhiteSpace(_url))
            {
                throw new ArgumentException(string.Format("Url ({0}) and Action ({1}) are both specified. Only one of both can be used.", _url, _action));
            }

            if (string.IsNullOrWhiteSpace(_action) && string.IsNullOrWhiteSpace(_url))
            {
                return "#";
            }

            return string.IsNullOrWhiteSpace(_action) ? _url : new UrlHelper(viewContext.RequestContext).Action(_action, _controller);
        }
    }

    public interface ISubMenuItem
    {
        string ToString(ViewContext viewContext);
    }

    public class SubMenuItem : ISubMenuItem
    {
        private string _text = string.Empty;
        private string _url = string.Empty;
        private string _controller = string.Empty;
        private string _action = string.Empty;
        internal HashSet<string> _roles = new HashSet<string>();

        public SubMenuItem Text(string text)
        {
            _text = text.Trim();
            return this;
        }

        public SubMenuItem Url(string url)
        {
            _url = url.Trim();
            return this;
        }

        public SubMenuItem Action<T>(string action) where T : IController
        {
            _action = action;
            _controller = typeof(T).Name.Replace("Controller", "");
            return this;
        }

        public SubMenuItem Roles(params string[] roles)
        {
            foreach (var role in roles)
            {
                _roles.Add(role);
            }
            return this;
        }

        public string ToString(ViewContext viewContext)
        {
            return string.Format("<li><a href=\"{0}\">{1}</a></li>", GetHyperLink(viewContext), _text);
        }

        private string GetHyperLink(ViewContext viewContext)
        {
            // Make sure either url is set or action, not both.
            if (!string.IsNullOrWhiteSpace(_action) && !string.IsNullOrWhiteSpace(_url))
            {
                throw new ArgumentException(string.Format("Url ({0}) and Action ({1}) are both specified. Only one of both can be used.", _url, _action));
            }

            if (string.IsNullOrWhiteSpace(_action) && string.IsNullOrWhiteSpace(_url))
            {
                return "#";
            }

            return string.IsNullOrWhiteSpace(_action) ? _url : new UrlHelper(viewContext.RequestContext).Action(_action, _controller);
        }
    }

    public class Divider : ISubMenuItem
    {
        public string ToString(ViewContext viewContext)
        {
            return "<li class=\"divider\"></li>";
        }
    }

    public class NavigationHeader : ISubMenuItem
    {
        private readonly string _headerText;

        public NavigationHeader(string headerText)
        {
            _headerText = headerText;
        }

        public string ToString(ViewContext viewContext)
        {
            return string.Format("<li class=\"dropdown-header\">{0}</li>", _headerText);
        }
    }
}
