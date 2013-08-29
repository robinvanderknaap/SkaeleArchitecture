using System;

namespace Data.Utils.QueryBuilder.SearchRules
{
    public interface ISearchRule
    {
        string Field { get; set; }
        string Statement { get; set; }
        Func<string, object> Action { get; set; }
    }
}