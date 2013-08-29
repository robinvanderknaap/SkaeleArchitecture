using System.Collections.Generic;
using MvcJqGrid;

namespace Infrastructure.Loggers
{
    public interface ILogRepository
    {
        IList<LogItem> GetLogItems(GridSettings gridSettings);
        long CountLogItems(GridSettings gridSettings);
    }
}
