using System;

namespace Infrastructure.FlashMessages
{
    [Serializable]
    public class FlashMessage
    {
        public FlashMessage(string title, string message, FlashMessageType messageType)
        {
            Title = title;
            Message = message;
            Type = messageType;
        }

        public string Title { get; private set; }
        public string Message { get; private set; }
        public FlashMessageType Type { get; private set; }
    }
}