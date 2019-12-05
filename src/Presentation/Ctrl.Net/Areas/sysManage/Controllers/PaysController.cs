using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Web;
using Ctrl.Core.Web.Attributes;
using Ctrl.System.Business;
using Ctrl.System.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Ctrl.Net.Areas.sysManage.Controllers
{
    /// <summary>
    ///  支付配置表控制器
    /// </summary>
    public class PaysController : BaseController
    {
        #region 构造函数

        private readonly ISystemPaysLogic _systemPaysLogic;


        public PaysController(ISystemPaysLogic systemPaysLogic)
        {
            _systemPaysLogic = systemPaysLogic;
        }

        #endregion

        #region 视图
        /// <summary>
        ///     列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统- 支付配置表-列表")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///     编辑
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统- 支付配置表-编辑")]
        [Permission("xtgl-zffs-SavePays")]
        public async Task<IActionResult> Edit(NullableIdInput input)
        {
            SystemPays pays = new SystemPays();
            if (!input.Id.IsNullOrEmptyGuid())
            {
                pays = await _systemPaysLogic.GetById(input.Id);
            }
            return View(pays);
        }
        #endregion

        #region 方法
        /// <summary>
        ///     获取 支付配置表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统- 支付配置表-方法-获取 支付配置表")]
        public async Task<JsonResult> GetList()
        {
            return Json((await _systemPaysLogic.GetAllEnumerableAsync()).OrderByDescending(o=>o.OrderNo));
        }
        /// <summary>
        ///     获取 支付配置表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统- 支付配置表-方法-保存支付配置")]
        [Permission("xtgl-zffs-SavePays")]
        public async Task<JsonResult> SavePays(SystemPays systemPays) {
            return Json(await _systemPaysLogic.SavePays(systemPays));
        }
        /// <summary>
        ///     获取 支付配置表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统- 支付配置表-方法-根据类型读取配置信息")]
        public async Task<JsonResult> GetPaysInfoByType(string TypeName) {
            return Json(await _systemPaysLogic.GetPaysInfoByType(TypeName));
        }

        #endregion
    }
}