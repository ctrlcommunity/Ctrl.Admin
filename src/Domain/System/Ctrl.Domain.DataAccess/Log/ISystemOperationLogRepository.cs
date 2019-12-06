using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.Domain.DataAccess.Log
{
    /// <summary>
    ///     操作日志数据访问层接口
    /// </summary>
    public interface ISystemOperationLogRepository:IRepository<SystemOperateLog>
    {
        /// <summary>
        ///     分页查询登录信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<PagedResults<SystemOperateLog>> GetPagingOperationLog(SystemLoginLogPagingInput queryParam);
    }
}
