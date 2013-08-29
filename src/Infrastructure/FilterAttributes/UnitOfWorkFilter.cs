using System.Web.Mvc;
using NHibernate;

namespace Infrastructure.FilterAttributes
{
    public class UnitOfWorkFilter : IActionFilter
    {
        private readonly ISession _session;

        public UnitOfWorkFilter(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Called before an action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _session.Transaction.Begin();
        }

        /// <summary>
        /// Called after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //var session = CurrentSessionContext.Unbind(_sessionFactory);
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                if (filterContext.Exception == null)
                {
                    _session.Transaction.Commit();
                }
                else
                {
                    _session.Transaction.Rollback();
                }
            }
        }
    }
}
