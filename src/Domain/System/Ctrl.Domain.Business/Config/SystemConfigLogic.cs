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
    ///     网站配置业务逻辑接口实现
    /// </summary>
    public class SystemConfigLogic:AsyncLogic<SystemConfig>,ISystemConfigLogic
    {
        #region 构造函数
        private readonly ISystemConfigRepository _systemConfigRepository;

        public SystemConfigLogic(ISystemConfigRepository systemConfigRepository):base(systemConfigRepository) {
            _systemConfigRepository = systemConfigRepository;
        }
        #endregion

        #region 方法
        /// <summary>
        ///     保存配置信息
        /// </summary>
        /// <param name="systemConfig"></param>
        /// <returns></returns>

        public Task<OperateStatus> SaveConfig(SystemConfig systemConfig)
        {
            if (systemConfig.Id.IsEmptyGuid())
            {
                systemConfig.Id = CombUtil.NewComb();
                systemConfig.CreateTime = DateTime.Now;
                return InsertAsync(systemConfig);
            }
            return UpdateAsync(systemConfig);
        }
        #endregion
    }
}