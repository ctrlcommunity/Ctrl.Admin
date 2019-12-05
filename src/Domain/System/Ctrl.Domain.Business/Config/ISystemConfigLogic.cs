using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Domain.Models.Dtos;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.System.Business
{
    /// <summary>
    /// 网站配置业务逻辑接口
    /// </summary>
    public interface ISystemConfigLogic: IAsyncLogic<SystemConfig>
    {
        /// <summary>
        ///     保存配置信息
        /// </summary>
        /// <returns></returns>
        Task<OperateStatus> SaveConfig(SystemConfig systemConfig);
    }
}
