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

        public FlashMessage(string title, string message, FlashMessageType messageType, string containerId)
        {
            Title = title;
            Message = message;
            Type = messageType;
            ContainerId = containerId; // Id of container the flashmessage has to be add to, instead of general container
        }

        public string Title { get; private set; }
        public string Message { get; private set; }
        public FlashMessageType Type { get; private set; }
        public string ContainerId { get; private set; }
    }
}