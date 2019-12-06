using System;
using System.Threading.Tasks;
using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;

namespace Ctrl.Domain.DataAccess.Log
{
    /// <summary>
    ///     操作日志数据访问实现
    /// </summary>
    public class SystemOperationLogRepository : PetaPocoRepository<SystemOperateLog>, ISystemOperationLogRepository
    {
        public Task<PagedResults<SystemOperateLog>> GetPagingOperationLog(SystemLoginLogPagingInput param)
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
            string sql = $"SELECT * FROM Sys_OperateLog where 1=1 {strWhere}";
            return SqlMapperUtil.PagingQuery<SystemOperateLog>(sql, param);
        }
    }
}
