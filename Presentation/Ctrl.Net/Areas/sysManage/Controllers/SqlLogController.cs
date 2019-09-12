using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Converts;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Domain.Business.Config;
using Ctrl.Domain.Models.Dtos.Config;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ctrl.System.Business;
using Ctrl.System.Models.Entities;
using Ctrl.Core.Web;

namespace Ctrl.Net.Areas.sysManage.Controllers
{
    /// <summary>
    /// sql执行日志表控制器
    /// </summary>
    public class  SqlLogController : BaseController
    {
        #region 构造函数

        private readonly ISystemSqlLogLogic _systemSqlLogLogic;


        public SqlLogController(ISystemSqlLogLogic systemSqlLogLogic)
        {
            _systemSqlLogLogic = systemSqlLogLogic;
        }

        #endregion

        #region 视图
		 /// <summary>
        ///     列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-sql执行日志表-列表")]
        public IActionResult Index()
        {
            return View();
        }
		 /// <summary>
        ///     编辑
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-sql执行日志表-编辑")]
        public IActionResult Edit()
        {
            return View();
        }
        #endregion

        #region 方法
		/// <summary>
        ///     获取sql执行日志表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-sql执行日志表-方法-获取sql执行日志表")]
        public async Task<JsonResult> GetList()
        {
            return Json(await _systemSqlLogLogic.GetAllEnumerableAsync());
        }
		#endregion
	}
}