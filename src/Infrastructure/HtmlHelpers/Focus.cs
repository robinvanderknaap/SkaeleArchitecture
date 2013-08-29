using System;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace Infrastructure.HtmlHelpers
{
    public static class FocusHelper
    {
        /// <summary>
        /// Strongly typed helper
        /// </summary>
        public static IHtmlString FocusFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            var inputName = ExpressionHelper.GetExpressionText(expression);
            return htmlHelper.Focus(inputName);
        }

        public static IHtmlString Focus(this HtmlHelper htmlHelper, string id)
        {
            return MvcHtmlString.Create("<script type=\"text/javascript\">$(document).ready(function() { $('#" + id + "').focus(); });</script>");
        }
        
    }
}
