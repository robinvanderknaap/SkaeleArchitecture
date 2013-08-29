using System;

namespace Data.Utils.QueryBuilder.SearchRules
{
    public class HavingSearchRule : ISearchRule
    {
        private Func<string, object> _action;

        public string Field { get; set; }
        public string Statement { get; set; }
        public Func<string, object> Action
        {
            get
            {
                return _action ?? (x => x);
            }
            set { _action = value; }
        }
    }
}
