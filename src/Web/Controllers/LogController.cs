using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Infrastructure.Loggers;
using MvcJqGrid;
using Web.Controllers.Base;

namespace Web.Controllers
{
    public class LogController : AuthorizedController
    {
        private readonly ILogRepository _logRepository;

        public LogController(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetLogData()
        {
            return Json(new{});
        }

        public ActionResult GetLogGridItems(GridSettings gridSettings)
        {
            var logGridItems = _logRepository.GetLogItems(gridSettings);
            var totalLogGridItems = _logRepository.CountLogItems(gridSettings);
            var pageIndex = gridSettings.PageIndex;
            var pageSize = gridSettings.PageSize;

            var jsonData = new
            {
                total = (totalLogGridItems / pageSize) + ((totalLogGridItems % pageSize > 0) ? 1 : 0),
                page = pageIndex,
                records = totalLogGridItems,
                rows = (from logItem in logGridItems
                        select new Dictionary<string, string>
                                            {
                                                { "Id", logItem.Id.ToString() },
                                                { "Created", logItem.Created.ToString() },
                                                { "Level", logItem.Level },
                                                { "Username", logItem.Username },
                                                { "Message", logItem.Message}
                                }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}
