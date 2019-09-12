using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.Domain.DataAccess.Log
{
    /// <summary>
    ///     登录日志数据访问
    /// </summary>
    public class SystemLoginLogRepository: PetaPocoRepository<SystemLoginLog>,ISystemLoginLogRepository
    {
        /// <summary>
        ///     获取一个月数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetDateMonth()
        {
            string sql = @"select convert(varchar(5),dateadd(dd,number,dateadd(month,-1,getdate())),110) as dt
from master..spt_values 
where type='P' and 
dateadd(dd,number,dateadd(month,-1,getdate()))<=getdate()";
            return SqlMapperUtil.Query<string>(sql);
        }

        /// <summary>
        /// 根据区域查询登录次数
        /// </summary>
        /// <param name="AreaName"></param>
        /// <returns></returns>
        public Task<int> GetLoginCountByAreaName(string AreaName)
        {
            string sql = $@"select count(1) from Sys_LoginLog
                            where IpAddressName like '%{AreaName}%'";
            return SqlMapperUtil.Count(sql);
        }
        /// <summary>
        ///     获取近一个月的登录记录
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SystemLoginLog>> GetLoginLogDateMonth()
        {
            string sql = @"select CreateTime from Sys_LoginLog
                where CreateTime>=dateadd(month,-1,getdate())";
            return SqlMapperUtil.Query<SystemLoginLog>(sql);
        }

        /// <summary>
        ///     分页查询登录信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemLoginLog>> PagingLoginLogQuery(SystemLoginLogPagingInput param)
        {
            string strWhere = "";
            if (!string.IsNullOrWhiteSpace(param.CreateUserCode))
            {
                strWhere+= $" AND CreateUserCode='{param.CreateUserCode}' ";
            }
            if (!string.IsNullOrWhiteSpace(param.CreateUserName))
            {
                strWhere += $" AND CreateUserName='{param.CreateUserName}' ";
            }
            if (param.startTime!=default(DateTime))
            {
                strWhere += $" AND CreateTime>='{param.startTime}' ";
            }
            if (param.endTime != default(DateTime))
            {
                strWhere += $" AND CreateTime<='{param.endTime}' ";
            }
            string sql = $"SELECT * FROM Sys_LoginLog where 1=1 {strWhere}";

            return SqlMapperUtil.PagingQuery<SystemLoginLog>(sql, param);
        }
    }
}
