using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;
using System;
using System.Threading.Tasks;

namespace Ctrl.Domain.DataAccess.Log
{
    /// <summary>
    ///     异常日志数据访问层实现
    /// </summary>
    public class SystemExceptionLogRepository: PetaPocoRepository<SystemExceptionLog>,ISystemExceptionLogRepository
    {
        /// <summary>
        ///     异常信息分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemExceptionLog>> PagingExceptionLogQuery(SystemLoginLogPagingInput param)
        {
            string strWhere = "";
            if (!string.IsNullOrWhiteSpace(param.CreateUserCode))
            {
                strWhere += $" AND CreateUserCode='{param.CreateUserCode}' ";
            }
            if (!string.IsNullOrWhiteSpace(param.CreateUserName))
            {
                strWhere += $" AND CreateUserName='{param.CreateUserName}' ";
            }
            if (param.startTime != default(DateTime))
            {
                strWhere += $" AND CreateTime>='{param.startTime}' ";
            }
            if (param.endTime != default(DateTime))
            {
                strWhere += $" AND CreateTime<='{param.endTime}' ";
            }
             string sql = $"select * from Sys_ExceptionLog where 1=1 {strWhere}";
            return SqlMapperUtil.PagingQuery<SystemExceptionLog>(sql, param);
        }
    }
}
