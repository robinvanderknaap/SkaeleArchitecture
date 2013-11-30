using System;
using System.Collections.Generic;
using Domain.Users;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using Infrastructure.DomainBase;
using Infrastructure.PasswordPolicies;

namespace Api.Core.Domain.ApiClients
{
    public class ApiClient : Entity, IClientDescription
    {
        private string _clientSecret;
        private readonly ClientType _clientType;
        private string _clientName;
        private readonly User _user;
        private readonly IList<ApiClientAuthorization> _clientAuthorizations = new List<ApiClientAuthorization>();
        private readonly string _clientIdentifier;
        private Uri _defaultCallback;

        public ApiClient(string clientName, string clientSecret, ClientType clientType, Uri defaultCallback, User user, IPasswordPolicy passwordPolicy)
        {
            if (!passwordPolicy.Validate(clientSecret))
            {
                throw new BusinessRuleViolationException("Client secret does not comply with policy");
            }
            
            ClientName = clientName;
            _clientSecret = clientSecret;
            _clientType = clientType;
            DefaultCallback = defaultCallback;
            _user = user.Required();

            _clientIdentifier = string.Format("{0}.{1}", clientName.Replace(" ", ""), Guid.NewGuid());
        }

        protected ApiClient() {}

        public string ClientIdentifier
        {
            get { return _clientIdentifier; }
        }

        public string ClientName
        {
            get { return _clientName; }
            set
            {
                _clientName = value.Required();
            }
        }

        public string ClientSecret
        {
            get { return _clientSecret; }
        }

        public ClientType ClientType
        {
            get { return _clientType; }
        }

        public Uri DefaultCallback
        {
            get { return _defaultCallback; }
            set { _defaultCallback = value.Required(); }
        }

        public User User
        {
            get { return _user; }
        }

        public IList<ApiClientAuthorization> ClientAuthorizations
        {
            get { return _clientAuthorizations; }
        }

        public void SetClientSecret(string clientSecret, IPasswordPolicy passwordPolicy)
        {
            if (!passwordPolicy.Validate(clientSecret))
            {
                throw new BusinessRuleViolationException("Client secret does not comply with policy");
            }

            _clientSecret = clientSecret;
        }

        public bool HasNonEmptySecret
        {
            get { return !string.IsNullOrEmpty(_clientSecret); }
        }

        public bool IsCallbackAllowed(Uri callback)
        {
            return callback.Scheme == DefaultCallback.Scheme &&
                        callback.Host == DefaultCallback.Host &&
                            callback.Port == DefaultCallback.Port;
        }

        public bool IsValidClientSecret(string secret)
        {
            return MessagingUtilities.EqualsConstantTime(secret, ClientSecret);
        }
    }
}
