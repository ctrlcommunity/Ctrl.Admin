using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.System.DataAccess;
using Ctrl.Domain.Models.Dtos;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;
using Ctrl.Core.Core.Utils;
using System;

namespace Ctrl.System.Business
{
    /// <summary>
    ///      支付配置表业务逻辑接口实现
    /// </summary>
    public class SystemPaysLogic:AsyncLogic<SystemPays>,ISystemPaysLogic
    {
        #region 构造函数
        private readonly ISystemPaysRepository _systemPaysRepository;

        public SystemPaysLogic(ISystemPaysRepository systemPaysRepository):base(systemPaysRepository) {
            _systemPaysRepository = systemPaysRepository;
        }
        /// <summary>
        ///     获取支付方式信息
        /// </summary>
        /// <returns></returns>
        public Task<SystemPays> GetPaysInfoByType(string TypeName)
        {
            return _systemPaysRepository.GetPaysInfoByType(TypeName);
        }
        #endregion

        #region 方法
        public async Task<OperateStatus> SavePays(SystemPays systemPays)
        {
            if (systemPays.PayId.IsEmptyGuid())
            {
                systemPays.PayId = CombUtil.NewComb();
                systemPays.CreateTime = DateTime.Now;
                return await InsertAsync(systemPays);
            }
            else
            {
                var pay = await _systemPaysRepository.GetById(systemPays.PayId);
                systemPays.CreateTime = pay.CreateTime;
                return await UpdateAsync(systemPays);
            }
        }
        #endregion
    }
}