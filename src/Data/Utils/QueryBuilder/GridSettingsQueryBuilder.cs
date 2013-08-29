using System.Collections.Generic;
using System.Linq;
using Data.Utils.QueryBuilder.SearchRules;
using MvcJqGrid;
using NHibernate;

namespace Data.Utils.QueryBuilder
{
    public class GridSettingsQueryBuilder
    {
        private readonly IList<string> _selectStatements = new List<string>();
        private readonly IList<string> _fromStatements = new List<string>();
        private readonly IList<string> _innerJoinStatements = new List<string>();
        private readonly IList<string> _leftJoinStatements = new List<string>();
        private readonly IList<string> _whereStatements = new List<string>();
        private readonly IList<string> _groupByStatements = new List<string>();
        private readonly IList<string> _havingStatements = new List<string>();
        private readonly IList<string> _orderByStatements = new List<string>();
        private readonly IList<ISearchRule> _searchRules = new List<ISearchRule>();
        private readonly IDictionary<string, string> _orderByRules = new Dictionary<string, string>();
        private string _defaultOrderBy;
        
        public GridSettingsQueryBuilder Select(string selectStatement)
        {
            _selectStatements.Add(selectStatement);
            return this;
        }

        public GridSettingsQueryBuilder Select(string[] selectStatements)
        {
            foreach (var selectStatement in selectStatements)
            {
                _selectStatements.Add(selectStatement);
            }
            return this;
        }

        public GridSettingsQueryBuilder From(string fromStatement)
        {
            _fromStatements.Add(fromStatement);
            return this;
        }

        public GridSettingsQueryBuilder From(string[] fromStatements)
        {
            foreach (var fromStatement in fromStatements)
            {
                _fromStatements.Add(fromStatement);
            }
            return this;
        }

        public GridSettingsQueryBuilder InnerJoin(string innerJoinStatement)
        {
            _innerJoinStatements.Add(innerJoinStatement);
            return this;
        }

        public GridSettingsQueryBuilder InnerJoin(string[] innerJoinStatements)
        {
            foreach (var innerJoinStatement in innerJoinStatements)
            {
                _innerJoinStatements.Add(innerJoinStatement);
            }
            return this;
        }

        public GridSettingsQueryBuilder LeftJoin(string leftJoinStatement)
        {
            _leftJoinStatements.Add(leftJoinStatement);
            return this;
        }

        public GridSettingsQueryBuilder LeftJoin(string[] leftJoinStatements)
        {
            foreach (var leftJoinStatement in leftJoinStatements)
            {
                _leftJoinStatements.Add(leftJoinStatement);
            }
            return this;
        }

        public GridSettingsQueryBuilder Where(string whereStatement)
        {
            _whereStatements.Add(whereStatement);
            return this;
        }

        public GridSettingsQueryBuilder Where(string[] whereStatements)
        {
            foreach (var whereStatement in whereStatements)
            {
                _whereStatements.Add(whereStatement);
            }
            return this;
        }

        public GridSettingsQueryBuilder GroupBy(string groupByStatement)
        {
            _groupByStatements.Add(groupByStatement);
            return this;
        }

        public GridSettingsQueryBuilder GroupBy(string[] groupByStatements)
        {
            foreach (var groupByStatement in groupByStatements)
            {
                _groupByStatements.Add(groupByStatement);
            }
            return this;
        }

        public GridSettingsQueryBuilder Having(string havingStatement)
        {
            _havingStatements.Add(havingStatement);
            return this;
        }

        public GridSettingsQueryBuilder Having(string[] havingStatements)
        {
            foreach (var havingStatement in havingStatements)
            {
                _havingStatements.Add(havingStatement);
            }
            return this;
        }

        public GridSettingsQueryBuilder OrderBy(string orderByStatement)
        {
            _orderByStatements.Add(orderByStatement);
            return this;
        }

        public GridSettingsQueryBuilder OrderBy(string[] orderByStatements)
        {
            foreach (var orderByStatement in orderByStatements)
            {
                _orderByStatements.Add(orderByStatement);
            }
            return this;
        }

        public GridSettingsQueryBuilder AddSearchRule(ISearchRule searchRule)
        {
            _searchRules.Add(searchRule);
            return this;
        }

        public GridSettingsQueryBuilder AddOrderByRule(string field, string orderByStatement)
        {
            _orderByRules.Add(field, orderByStatement);
            return this;
        }

        public GridSettingsQueryBuilder SetDefaultOrderBy(string orderByStatement)
        {
            _defaultOrderBy = orderByStatement;
            return this;
        }

        public string ToString(GridSettings gridSettings)
        {
            return GetQueryBuilder(gridSettings).ToString();
        }

        public ISQLQuery ToSqlQuery(GridSettings gridSettings, ISession session)
        {
            return GetQueryBuilder(gridSettings).ToSqlQuery(session);
        }

        public ISQLQuery ToSqlCountQuery(GridSettings gridSettings, ISession session)
        {
            return GetQueryBuilder(gridSettings).ToSqlCountQuery(session);
        }

        private QueryBuilder GetQueryBuilder(GridSettings gridSettings)
        {
            var queryBuilder = new QueryBuilder()
                .Select(_selectStatements.ToArray())
                .From(_fromStatements.ToArray())
                .InnerJoin(_innerJoinStatements.ToArray())
                .LeftJoin(_leftJoinStatements.ToArray())
                .Where(_whereStatements.ToArray())
                .GroupBy(_groupByStatements.ToArray())
                .Having(_havingStatements.ToArray())
                .OrderBy(_orderByStatements.ToArray());
            
            if (gridSettings.IsSearch)
            {
                foreach (var rule in gridSettings.Where.rules)
                {
                    var searchRule = _searchRules.SingleOrDefault(x => x.Field == rule.field);
                    if (searchRule == null) continue; // When a rule is supplied, this doesn't always mean a WhereSearchRule is specified
                    if (searchRule.GetType() == typeof(WhereSearchRule))
                        queryBuilder.Where(string.Format(searchRule.Statement, ":" + searchRule.Field));
                    if (searchRule.GetType() == typeof(HavingSearchRule))
                        queryBuilder.Having(string.Format(searchRule.Statement, ":" + searchRule.Field));
                    queryBuilder.AddParameter(searchRule.Field, searchRule.Action.Invoke(rule.data));
                }
            }

            if (!string.IsNullOrWhiteSpace(gridSettings.SortColumn) && _orderByRules.Count > 0)
            {
                var orderByRule = _orderByRules.Single(x => x.Key == gridSettings.SortColumn);
                queryBuilder.OrderBy(string.Format(orderByRule.Value, gridSettings.SortOrder));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(_defaultOrderBy))
                {
                    queryBuilder.OrderBy(_defaultOrderBy);
                }
            }

            return queryBuilder;
        }
    }
}
