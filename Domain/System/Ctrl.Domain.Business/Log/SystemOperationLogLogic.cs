using System.Threading.Tasks;
using Ctrl.Core.Business;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.DataAccess.Log;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;

namespace Ctrl.Domain.Business.Log
{
    public class SystemOperationLogLogic : AsyncLogic<SystemOperateLog>, ISystemOperationLogLogic
    {
        #region 构造函数

        private readonly ISystemOperationLogRepository _repository;

        public SystemOperationLogLogic(ISystemOperationLogRepository operationLogRepository)
            : base(operationLogRepository)
        {
            _repository = operationLogRepository;
        }
        #endregion
        /// <summary>
        ///     分页
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public  Task<PagedResults<SystemOperateLog>> GetPagingOperationLog(SystemLoginLogPagingInput queryParam)
        {
            return  _repository.GetPagingOperationLog(queryParam);
        }
    }
}
