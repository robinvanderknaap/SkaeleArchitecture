using System;
using System.Collections.Generic;

namespace Api.Core.Domain.ApiClients
{
    public class ApiClientAuthorization
    {
        public DateTime IssueDate { get; set; }

        public string UserId { get; set; }

        public HashSet<string> Scope { get; set; }

        public DateTime? ExpirationDateUtc { get; set; }
    }
}