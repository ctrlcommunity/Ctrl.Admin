using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.System.DataAccess;
using Ctrl.Domain.Models.Dtos;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.System.Business
{
    /// <summary>
    ///     sql执行日志表业务逻辑接口实现
    /// </summary>
    public class SystemSqlLogLogic:AsyncLogic<SystemSqlLog>,ISystemSqlLogLogic
    {
        #region 构造函数
        private readonly ISystemSqlLogRepository _systemSqlLogRepository;

        public SystemSqlLogLogic(ISystemSqlLogRepository systemSqlLogRepository):base(systemSqlLogRepository) {
            _systemSqlLogRepository = systemSqlLogRepository;
        }

        #endregion
	}
}