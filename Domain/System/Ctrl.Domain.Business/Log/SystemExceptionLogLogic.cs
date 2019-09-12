using Ctrl.Core.Business;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.DataAccess.Log;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.Domain.Business.Log
{
    public class SystemExceptionLogLogic : AsyncLogic<SystemExceptionLog>, ISystemExceptionLogLogic
    {
        #region 构造函数
        private readonly ISystemExceptionLogRepository _exceptionLogRepository;
        public SystemExceptionLogLogic(ISystemExceptionLogRepository exceptionLogRepository)
            : base(exceptionLogRepository)
        {
            this._exceptionLogRepository = exceptionLogRepository;
        }
        /// <summary>
        ///     异常信息分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemExceptionLog>> PagingExceptionLogQuery(SystemLoginLogPagingInput query)
        {
            return  _exceptionLogRepository.PagingExceptionLogQuery(query);
        }
        #endregion
    }
}
