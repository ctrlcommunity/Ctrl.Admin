using Ctrl.Core.Business;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.Domain.Business.Log
{
    public interface ISystemOperationLogLogic: IAsyncLogic<SystemOperateLog>
    {
        /// <summary>
        ///     分页查询登录日志
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<PagedResults<SystemOperateLog>> GetPagingOperationLog(SystemLoginLogPagingInput queryParam);
    }
}
