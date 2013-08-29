using System;
using System.Collections.Generic;
using Data.Utils.QueryBuilder;
using Data.Utils.QueryBuilder.SearchRules;
using Infrastructure.Loggers;
using MvcJqGrid;
using NHibernate;
using NHibernate.Transform;

namespace Data
{
    public class LogRepository : ILogRepository
    {
        private readonly ISession _session;

        public LogRepository(ISession session)
        {
            _session = session;
        }

        public IList<LogItem> GetLogItems(GridSettings gridSettings)
        {
            return LogItemsQueryBuilder().ToSqlQuery(gridSettings, _session)
                .AddScalar("Id", NHibernateUtil.Guid)
                .AddScalar("Created", NHibernateUtil.DateTime)
                .AddScalar("Level", NHibernateUtil.String)
                .AddScalar("Environment", NHibernateUtil.String)
                .AddScalar("Source", NHibernateUtil.String)
                .AddScalar("Message", NHibernateUtil.String)
                .AddScalar("Details", NHibernateUtil.String)
                .AddScalar("Username", NHibernateUtil.String)
                .AddScalar("RequestMethod", NHibernateUtil.String)
                .AddScalar("RequestUrl", NHibernateUtil.String)
                .AddScalar("UrlReferrer", NHibernateUtil.String)
                .AddScalar("ClientBrowser", NHibernateUtil.String)
                .AddScalar("IpAddress", NHibernateUtil.String)
                .AddScalar("PostedFormValues", NHibernateUtil.String)
                .AddScalar("Stacktrace", NHibernateUtil.String)
                .AddScalar("Exception", NHibernateUtil.String)
                .SetResultTransformer(Transformers.AliasToBean(typeof(LogItem)))
                .SetFirstResult((gridSettings.PageIndex) * gridSettings.PageSize)
                .SetMaxResults(gridSettings.PageSize)
                .List<LogItem>();
        }

        public long CountLogItems(GridSettings gridSettings)
        {
            return LogItemsQueryBuilder()
                .ToSqlCountQuery(gridSettings, _session)
                .UniqueResult<long>();
        }

        private static GridSettingsQueryBuilder LogItemsQueryBuilder()
        {
            var query = new GridSettingsQueryBuilder()
                .Select(@"
                    Id,
	                Created,
	                Level,
	                Environment,
	                Source,
	                Message,
	                Details,
	                Username,
	                RequestMethod,
	                RequestUrl,
	                UrlReferrer,
	                ClientBrowser,
	                IpAddress,
	                PostedFormValues,
	                Stacktrace,
	                Exception"
                )
                .From("Log")
                .AddSearchRule(new WhereSearchRule {Field = "Id", Statement = "Id = {0}"})
                .AddSearchRule(new WhereSearchRule { Field = "Created", Statement = "Created = {0}", Action = s => DateTime.Parse(s) })
                .AddSearchRule(new WhereSearchRule { Field = "Level", Statement = "Level = {0}" })
                .AddSearchRule(new WhereSearchRule { Field = "Username", Statement = "Username like {0}", Action = s => "%" + s + "%" })
                .AddSearchRule(new WhereSearchRule { Field = "Message", Statement = "Message like {0}", Action = s => "%" + s + "%" })
                .AddOrderByRule("Id", "Id {0}")
                .AddOrderByRule("Created", "Created {0}")
                .AddOrderByRule("Level", "Level {0}")
                .AddOrderByRule("Username", "Username {0}")
                .AddOrderByRule("Message", "Message {0}")
                .SetDefaultOrderBy("Created DESC")
            ;
                
            return query;
        }
    }
}
