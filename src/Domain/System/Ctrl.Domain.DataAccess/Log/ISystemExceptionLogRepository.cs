using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.Domain.DataAccess.Log
{
    /// <summary>
    ///     异常日志数据访问层接口
    /// </summary>
    public interface ISystemExceptionLogRepository : IRepository<SystemExceptionLog>
    {
        Task<PagedResults<SystemExceptionLog>> PagingExceptionLogQuery(SystemLoginLogPagingInput query);
    }
}
