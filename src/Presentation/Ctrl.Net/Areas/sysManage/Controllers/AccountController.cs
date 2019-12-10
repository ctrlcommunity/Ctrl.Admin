using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Auth;
using Ctrl.Core.Core.Log;
using Ctrl.Core.Core.Security;
using Ctrl.Core.Web;
using Ctrl.Core.Web.Attributes;
using Ctrl.Domain.Business.Identity;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Entities;
using Ctrl.Domain.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Ctrl.Net.Areas.SysManage.Controllers
{
    /// <summary>
    ///     登录控制器
    /// </summary>
    [Ignore]
    public class AccountController : BaseController
    {
        #region  构造函数
        private readonly ISystemUserLogic _systemUserLogic;


        public AccountController(ISystemUserLogic systemUserLogic)
        {
            _systemUserLogic = systemUserLogic;
        }
        #endregion

        #region 视图
        [SkipPermission]
        public  ActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// 登录退出界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            AuthenticationExtension.SignOut();
            return RedirectToAction("Login", "Account");
        }
        /// <summary>
        ///     个人资料
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PerInfo()
        {
            SystemUser user = new SystemUser();
            user = await _systemUserLogic.GetById(CurrentUser.UserId);
            return View(user);
        }
        /// <summary>
        ///     绑定账号
        /// </summary>
        /// <returns></returns>
        public ActionResult BindAccount()
        {
            return View();
        }

        #endregion

        #region 方法
        /// <summary>
        ///     登录
        /// </summary>
        /// <param name="input">登录参数</param>
        /// <returns></returns>
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<JsonResult> Submit(UserLoginInput model)
        {
            model.Password = _3DESEncrypt.Encrypt(model.Password);
            var info = await _systemUserLogic.CheckUserByCodeAndPwdAsync(model);
            if (info.Data != null)
            {
                var prin = new PrincipalUser()
                {
                    UserId = Guid.Parse(info.Data.UserId),
                    Code = info.Data.Code,
                    Name = info.Data.Name,
                    IsAdmin = info.Data.IsAdmin,
                    RoleName = info.Data.RoleName,
                    ImgUrl=info.Data.ImgUrl
                };
                if (prin.Code=="admin")
                {
                    prin.RoleName = "超级管理员";
                }
                //写入Cookie信息
                AuthenticationExtension.SetAuthCookie(prin);

                //写入日志
                var logHandler = new LoginLogHandler(info.Data.UserId, info.Data.Code, info.Data.Name, (int)EnumLoginType.账号密码登录);
                logHandler.WriteLog();
            }
            return Json(info);
        }

        #endregion
    }


}