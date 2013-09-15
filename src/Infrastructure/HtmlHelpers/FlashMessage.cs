using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.FlashMessages;

namespace Infrastructure.HtmlHelpers
{
    /// <summary>
    /// Flashmessage helper based on twitter bootstrap alerts
    /// </summary>
    public static class FlashMessageHelper
    {
        public static IHtmlString FlashMessage(this HtmlHelper helper, string id)
        {
            // Return empty string when no flashmessages were found in either viewdata or tempdata
            if (helper.ViewContext.TempData["flashMessage"] == null && helper.ViewContext.ViewData["flashMessage"] == null)
            {
                return new HtmlString(string.Format("<div class=\"col-md-12\" id='{0}'></div>", id));
            }

            // Get flashmessages from tempdata else from viewdata
            var flashMessages = (List<FlashMessage>)helper.ViewContext.TempData["flashMessage"] 
                                    ?? (List<FlashMessage>)helper.ViewContext.ViewData["flashMessage"];

            // Create div tag, used as container for all flashmessages
            var container = new TagBuilder("div");
            
            // Set container id
            container.Attributes.Add("id", id);
            container.AddCssClass("col-md-12");
            
            var generalFlashMessages = flashMessages.Where(x => string.IsNullOrWhiteSpace(x.ContainerId)).ToList();
            var inlineFlashMessages = flashMessages.Where(x => !string.IsNullOrWhiteSpace(x.ContainerId)).ToList();

            // Add flashmessages to container
            foreach(var flashMessage in generalFlashMessages)
            {
                const string flashMessageTemplate = @"<div class=""{0}""  onclick='$(this).slideUp()'>{1}{2}</div>";
                
                // Add flashmessage to container
                container.InnerHtml += string.Format(
                    flashMessageTemplate,
                    GetCssClass(flashMessage),
                    string.IsNullOrWhiteSpace(flashMessage.Title) ? string.Empty : "<strong>" + flashMessage.Title + " - </strong>",
                    flashMessage.Message
                );
            }

            var scriptTemplate = "";

            if (inlineFlashMessages.Any())
            {
                var flashMessageTemplates = new List<string>();
                
                foreach (var flashMessage in inlineFlashMessages)
                {
                    const string flashMessageTemplate = @"$('#{0}').append('<div class=""{1}"" onclick=""$(this).slideUp()"">{2}{3}</div>');";

                    flashMessageTemplates.Add(string.Format(
                        flashMessageTemplate,
                        flashMessage.ContainerId,
                        GetCssClass(flashMessage),
                        string.IsNullOrWhiteSpace(flashMessage.Title)
                            ? string.Empty
                            : "<strong>" + flashMessage.Title + " - </strong>".Replace("'", "&lsquo;"),
                        flashMessage.Message.Replace("'", "&lsquo;")
                    ));
                }

                scriptTemplate = string.Format(@"<script type=""text/javascript"">$(function(){{ {0} }});</script>", string.Join("", flashMessageTemplates));
            }

            return new HtmlString(container + scriptTemplate);
        }

        private static string GetCssClass(FlashMessage flashMessage)
        {
            // Default to info css class
            var cssClass = "message-info";

            // Determine class name based on message type
            switch (flashMessage.Type)
            {
                case FlashMessageType.Info:
                    cssClass = "alert alert-info alert-block";
                    break;
                case FlashMessageType.Success:
                    cssClass = "alert alert-success alert-block";
                    break;
                case FlashMessageType.Warning:
                    cssClass = "alert alert-block";
                    break;
                case FlashMessageType.Error:
                    cssClass = "alert alert-danger alert-block";
                    break;
            }
            return cssClass;
        }
    }
}