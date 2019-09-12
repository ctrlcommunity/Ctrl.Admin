using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.Domain.DataAccess.Log
{
    /// <summary>
    ///     登录日志数据访问层接口
    /// </summary>
    public interface ISystemLoginLogRepository: IRepository<SystemLoginLog>
    {
        /// <summary>
        ///     分页查询登录信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<PagedResults<SystemLoginLog>> PagingLoginLogQuery(SystemLoginLogPagingInput param);
        /// <summary>
        ///     根据区域查询登录次数
        /// </summary>
        /// <param name="AreaName"></param>
        /// <returns></returns>
        Task<int> GetLoginCountByAreaName(string AreaName);
        /// <summary>
        ///     获取一个月数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetDateMonth();
        /// <summary>
        ///     获取一个月的登录记录
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SystemLoginLog>> GetLoginLogDateMonth();



    }
}
