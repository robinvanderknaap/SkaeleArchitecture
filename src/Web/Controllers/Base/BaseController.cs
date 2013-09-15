using System.Collections.Generic;
using System.Web.Mvc;
using Infrastructure.FlashMessages;

namespace Web.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        private readonly IList<FlashMessage> _flashMessages = new List<FlashMessage>();

        /// <summary>
        /// Adds flashmessage to page
        /// </summary>
        /// <remarks>Mulitple flashmessages can be added to the page using this method</remarks>
        protected void AddFlashMessage(string title, string message, FlashMessageType flashMessageType)
        {
            _flashMessages.Add(new FlashMessage(title, message, flashMessageType));

            ViewData["flashMessage"] = _flashMessages;
            TempData["flashMessage"] = _flashMessages;
        }

        /// <summary>
        /// Adds flashmessage to page in specified container
        /// </summary>
        /// <remarks>Mulitple flashmessages can be added to the page using this method</remarks>
        protected void AddFlashMessage(string title, string message, FlashMessageType flashMessageType, string containerId)
        {
            _flashMessages.Add(new FlashMessage(title, message, flashMessageType, containerId));

            ViewData["flashMessage"] = _flashMessages;
            TempData["flashMessage"] = _flashMessages;
        }
    }
}
