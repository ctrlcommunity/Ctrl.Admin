using Ctrl.Core.Business;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.Domain.Business.Log
{
    /// <summary>
    ///     登录日志业务逻辑接口
    /// </summary>
    public interface ISystemLoginLogLogic :IAsyncLogic<SystemLoginLog>
    {
        /// <summary>
        ///     分页查询登录信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<PagedResults<SystemLoginLog>> PagingLoginLogQuery(SystemLoginLogPagingInput logPagingInput);
        /// <summary>
        ///     登录数据分析图数据
        /// </summary>
        /// <returns></returns>
        Task<List<LoginDataOutPut>> GetLoginCountData();
        /// <summary>
        ///     获取登录日志分析数据
        /// </summary>
        /// <returns></returns>
        Task<LoginDataAnalysisOutPut> FindLoginLogAnalysis();
    }
}
