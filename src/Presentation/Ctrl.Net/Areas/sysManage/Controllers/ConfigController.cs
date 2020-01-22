using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Web;
using Ctrl.System.Business;
using Ctrl.System.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Ctrl.Net.Areas.sysManage.Controllers
{
    /// <summary>
    /// 网站配置控制器
    /// </summary>
    public class  ConfigController : BaseController
    {
        #region 构造函数

        private readonly ISystemConfigLogic _systemConfigLogic;


        public ConfigController(ISystemConfigLogic systemConfigLogic)
        {
            _systemConfigLogic = systemConfigLogic;
        }

        #endregion

        #region 视图
		 /// <summary>
        ///     列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-网站配置-配置信息")]
        public async Task<IActionResult> Index()
        {
            SystemConfig config = (await _systemConfigLogic.GetAllEnumerableAsync()).ToList().FirstOrDefault();
            if (config==default(SystemConfig))
            {
                config = new SystemConfig();
            }
            return View(config);
        }
        #endregion

        #region 方法
        /// <summary>
        /// /   
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("网站配置-方法-新增/编辑-保存")]
        public async Task<JsonResult> SaveConfig(SystemConfig system) {
            return Json(await _systemConfigLogic.SaveConfig(system));
        }
		#endregion
	}
}