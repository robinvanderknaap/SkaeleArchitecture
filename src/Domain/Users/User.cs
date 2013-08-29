using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using Domain.AbstractRepository;
using Infrastructure.ApplicationSettings;
using Infrastructure.DomainBase;

namespace Domain.Users
{
    /// <summary>
    /// Represents application user
    /// By implementing IPrincipal we open the possibiity to attach the user entity to the current thread.
    /// This also enables default security features in .NET applications.
    /// </summary>
    public class User : Entity, IPrincipal
    {
        private CultureInfo _culture;
        private string _email;
        private string _displayName;
        private readonly ICollection<Role> _roles = new HashSet<Role>();
        private IIdentity _identity;
       
        // We need a user repository to make sure the email is unique
        // We need application settings to determine if culture is valid
        public User(string email, string displayName, CultureInfo culture, IUserRepository userRepository, IApplicationSettings applicationSettings)
        {
            SetCulture(culture, applicationSettings);
            SetEmail(email, userRepository);
            DisplayName = displayName;
            
            // Users are active by default
            IsActive = true;
        }

        /// <summary>
        /// Parameterless constructor used by Nhibernate
        /// </summary>
        protected User() { }

	    /// <summary>
	    /// Email is used as login name, therefore must be unique
	    /// </summary>
        public virtual string Email
	    {
		    get { return _email;}
	    }

        public virtual string DisplayName
        {
	        get { return _displayName;}
	        set 
            { 
                _displayName = value.Required("Display name is required");
            }
        }

        public virtual CultureInfo Culture
        {
            get { return _culture; }
        }

        public virtual ICollection<Role> Roles
        {
            get { return _roles; }
        }

        public virtual bool IsActive { get; set; }

        public virtual IIdentity Identity
        {
            get
            {
                return _identity ?? (_identity = new GenericIdentity(Email));
            }
        }
        
        /// <summary>
        /// Set email, will perform unique check when email is changed
        /// </summary>
        public virtual void SetEmail(string email, IUserRepository userRepository)
        {
            // Validate email address
            email
                .Required("Email address is required")
                .ValidEmailAddress();

            // Make sure emailaddress is unique when changed
            if (_email != email && !userRepository.IsUserEmailUnique(email))
            {
                throw new BusinessRuleViolationException("Email must be unique");
            }

            _email = email;
        }

        /// <summary>
        /// Set culture, will perform check if culture is supported
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="applicationSettings"></param>
        public virtual void SetCulture(CultureInfo culture, IApplicationSettings applicationSettings)
        {
            culture.Required("Culture is required");

            if (!applicationSettings.AcceptedCultures.Contains(culture))
            {
                throw new BusinessRuleViolationException("Culture is not accepted");
            }
            _culture = culture;
        }

        public virtual bool IsInRole(Role role)
        {
            return _roles.Contains(role);
        }

        public virtual bool IsInRole(string role)
        {
            return IsInRole((Role)Enum.Parse(typeof(Role), role, true));
        }
    }
}