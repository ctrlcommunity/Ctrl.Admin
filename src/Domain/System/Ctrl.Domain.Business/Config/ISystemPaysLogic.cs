using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Domain.Models.Dtos;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.System.Business
{
    /// <summary>
    ///  支付配置表业务逻辑接口
    /// </summary>
    public interface ISystemPaysLogic: IAsyncLogic<SystemPays>
    {
        /// <summary>
        ///     保存支付配置
        /// </summary>
        /// <param name="systemPays"></param>
        /// <returns></returns>
        Task<OperateStatus> SavePays(SystemPays systemPays);
        /// <summary>
        ///     获取支付方式信息
        /// </summary>
        /// <returns></returns>
        Task<SystemPays> GetPaysInfoByType(string TypeName);
    }
}
