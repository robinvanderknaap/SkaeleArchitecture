using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace Data.Utils.QueryBuilder
{
    public class QueryBuilder
    {
        private readonly IList<string> _selectStatements = new List<string>();
        private readonly IList<string> _fromStatements = new List<string>();
        private readonly IList<string> _joinStatements = new List<string>();
        private readonly IList<string> _whereStatements = new List<string>();
        private readonly IList<string> _groupByStatements = new List<string>();
        private readonly IList<string> _havingStatements = new List<string>();
        private readonly IList<string> _orderByStatements = new List<string>();
        private readonly IDictionary<string, object> _parameters = new Dictionary<string, object>();

        public QueryBuilder Select(string selectStatement)
        {
            _selectStatements.Add(selectStatement);
            return this;
        }

        public QueryBuilder Select(string[] selectStatements)
        {
            foreach (var selectStatement in selectStatements)
            {
                _selectStatements.Add(selectStatement);
            }
            return this;
        }

        public QueryBuilder From(string fromStatement)
        {
            _fromStatements.Add(fromStatement);
            return this;
        }

        public QueryBuilder From(string[] fromStatements)
        {
            foreach (var fromStatement in fromStatements)
            {
                _fromStatements.Add(fromStatement);
            }
            return this;
        }

        public QueryBuilder InnerJoin(string innerJoinStatement)
        {
            _joinStatements.Add("inner join\r\n\t" + innerJoinStatement);
            return this;
        }

        public QueryBuilder InnerJoin(string[] innerJoinStatements)
        {
            foreach (var innerJoinStatement in innerJoinStatements)
            {
                _joinStatements.Add("inner join\r\n\t" + innerJoinStatement);
            }
            return this;
        }

        public QueryBuilder LeftJoin(string leftJoinStatement)
        {
            _joinStatements.Add("left join\r\n\t" + leftJoinStatement);
            return this;
        }

        public QueryBuilder LeftJoin(string[] leftJoinStatements)
        {
            foreach (var leftJoinStatement in leftJoinStatements)
            {
                _joinStatements.Add("left join\r\n\t" + leftJoinStatement);
            }
            return this;
        }

        public QueryBuilder Where(string whereStatement)
        {
            _whereStatements.Add(whereStatement);
            return this;
        }

        public QueryBuilder Where(string whereStatement,  Func<string, object> action, string value)
        {
            var randomString = Guid.NewGuid().ToString("N");

            Where(string.Format(whereStatement, ":" + randomString));

            AddParameter(randomString, action.Invoke(value));

            return this;
        }

        public QueryBuilder Where(string whereStatement, string value)
        {
            var randomString = Guid.NewGuid().ToString("N");

            Where(string.Format(whereStatement, ":" + randomString));

            AddParameter(randomString, value);

            return this;
        }

        public QueryBuilder Where(string[] whereStatements)
        {
            foreach (var whereStatement in whereStatements)
            {
                _whereStatements.Add(whereStatement);
            }
            return this;
        }

        public QueryBuilder GroupBy(string groupByStatement)
        {
            _groupByStatements.Add(groupByStatement);
            return this;
        }

        public QueryBuilder GroupBy(string[] groupByStatements)
        {
            foreach (var groupByStatement in groupByStatements)
            {
                _groupByStatements.Add(groupByStatement);
            }
            return this;
        }

        public QueryBuilder Having(string havingStatement)
        {
            _havingStatements.Add(havingStatement);
            return this;
        }

        public QueryBuilder Having(string[] havingStatements)
        {
            foreach (var havingStatement in havingStatements)
            {
                _havingStatements.Add(havingStatement);
            }
            return this;
        }

        public QueryBuilder OrderBy(string orderByStatement)
        {
            _orderByStatements.Add(orderByStatement);
            return this;
        }

        public QueryBuilder OrderBy(string[] orderByStatements)
        {
            foreach (var orderByStatement in orderByStatements)
            {
                _orderByStatements.Add(orderByStatement);
            }
            return this;
        }

        public QueryBuilder AddParameter(string name, object value)
        {
            _parameters.Add(name, value);
            return this;
        }

        public override string ToString()
        {
            var query = new StringBuilder();

            // Select statements
            query.AppendLine("select");
            query.AppendLine("\t" + string.Join(",\r\n\t", _selectStatements));

            // From statements
            query.AppendLine("from");
            query.AppendLine("\t" + string.Join(",\r\n\t", _fromStatements));

            // Join statements
            query.AppendLine(string.Join("\r\n", _joinStatements));

            // Where statements
            if (_whereStatements.Count != 0)
            {
                query.AppendLine("where");
                query.AppendLine("\t" + string.Join(" and\r\n\t", _whereStatements));
            }

            // GroupBy statements
            if (_groupByStatements.Count != 0)
            {
                query.AppendLine("group by");
                query.AppendLine("\t" + string.Join(",\r\n\t", _groupByStatements));
            }

            // Having statements
            if (_havingStatements.Count != 0)
            {
                query.AppendLine("having");
                query.AppendLine("\t" + string.Join(" and\r\n\t", _havingStatements));
            }

            // OrderBy statements
            if (_orderByStatements.Count != 0)
            {
                query.AppendLine("order by");
                query.AppendLine("\t" + string.Join(",\r\n\t", _orderByStatements));
            }

            return query.ToString();
        }

        public ISQLQuery ToSqlQuery(ISession session)
        {
            var query = session
                .CreateSQLQuery(ToString());

            foreach (var parameter in _parameters)
            {
                query.SetParameter(parameter.Key, parameter.Value);
            }

            return query;
        }

        public ISQLQuery ToSqlCountQuery(ISession session)
        {
            var stringBuilder = new StringBuilder();

            // Select statements
            stringBuilder.AppendLine("select");
            stringBuilder.AppendLine("\t count(*) \r\n\t");

            // From statements
            stringBuilder.AppendLine("from");
            stringBuilder.AppendLine("\t" + string.Join(",\r\n\t", _fromStatements));

            // Join statements
            stringBuilder.AppendLine(string.Join("\r\n", _joinStatements));

            // Where statements
            if (_whereStatements.Count != 0)
            {
                stringBuilder.AppendLine("where");
                stringBuilder.AppendLine("\t" + string.Join(" and\r\n\t", _whereStatements));
            }

            // Having statements
            if (_havingStatements.Count != 0)
            {
                stringBuilder.AppendLine("having");
                stringBuilder.AppendLine("\t" + string.Join(" and\r\n\t", _havingStatements));
            }

            var query = session
                .CreateSQLQuery(stringBuilder.ToString());

            foreach (var parameter in _parameters)
            {
                query.SetParameter(parameter.Key, parameter.Value);
            }

            return query;
        }
    }
}
