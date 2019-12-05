using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Web;
using Ctrl.Core.Web.Attributes;
using Ctrl.Domain.Business.Identity;
using Ctrl.Domain.Models.Dtos.Identity;
using Ctrl.Domain.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Ctrl.Net.Areas.sysManage.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    public class UserController : BaseController
    {
        #region 构造函数
        private readonly ISystemUserLogic _systemUserLogic;
        public UserController(ISystemUserLogic systemUserLogic)
        {
            _systemUserLogic = systemUserLogic;
        }

        #endregion

        #region 视图
        /// <summary>
        ///     列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-用户-列表")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///     编辑
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-用户-编辑")]
        [Permission("xtgl-xtyh-SaveUser")]
        public IActionResult Edit() {
            return View();
        }
        #endregion

        #region 方法

        [HttpPost]
        [CreateBy("冯辉")]
        [Description("用户列表-方法-列表-获取用户列表")]
        public async Task<JsonResult> GetPagingUser(QueryParam queryParam) {
            return JsonForGridPaging(await _systemUserLogic.GetPagingSysUser(queryParam));
        }
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("用户列表-方法-保存用户")]
        [Permission("xtgl-xtyh-SaveUser")]
        public async Task<JsonResult> SaveUser(SystemUserSaveInput input)
        {
            return Json(await _systemUserLogic.SaveUser(input));
        }
        /// <summary>
        ///     检测代码是否已经具有重复项
        /// </summary>
        /// <param name="input">需要验证的参数</param>
        /// <returns></returns>
        [HttpGet]
        [CreateBy("冯辉")]
        [Description("用户维护-方法-新增/编辑-检测代码是否已经具有重复项")]
        public async Task<JsonResult> CheckUserCode(CheckSameValueInput input) {
            input.Id = CurrentUser.UserId;
            return Json(await _systemUserLogic.CheckUserCode(input));
        }

        #endregion
    }
}