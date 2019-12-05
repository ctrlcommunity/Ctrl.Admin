using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Web;
using Ctrl.Domain.Business.Log;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Ctrl.Net.Areas.SysManage.Controllers
{
    /// <summary>
    ///     日志控制器
    /// </summary>
    public class LogController : BaseController
    {

        #region 构造函数
        private readonly ISystemLoginLogLogic _loginLogLogic;
        private readonly ISystemOperationLogLogic _operationLogLogic;
        private readonly ISystemExceptionLogLogic _exceptionLogLogic;
        public LogController(ISystemLoginLogLogic loginLogLogic,ISystemOperationLogLogic operationLogLogic, ISystemExceptionLogLogic exceptionLogLogic)
        {
            this._loginLogLogic = loginLogLogic;
            this._operationLogLogic = operationLogLogic;
            this._exceptionLogLogic = exceptionLogLogic;
           
        }
        #endregion

        #region 登陆日志
        /// <summary>
        ///     登录日志
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("登录日志-列表")]
        public ActionResult Login()
        {
            return View();
        }
        /// <summary>
        ///     登录日志分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("登录日志-方法-列表-获取所有登录日志信息")]
        public async Task<JsonResult> GetPagingLoginLog(SystemLoginLogPagingInput logPagingInput)
        {
            return JsonForGridPaging(await _loginLogLogic.PagingLoginLogQuery(logPagingInput));
        }

        /// <summary>
        ///     登录日志
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("登录日志-数据分析")]
        public ActionResult LoginData() {
            return View();
        }
        /// <summary>
        ///     获取用户登录次数地区分部图
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetLoginCountData() {
            return Json(await _loginLogLogic.GetLoginCountData());
        }
        /// <summary>
        ///     获取登录日志分析数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("登录日志-数据日志数据分析")]
        public async Task<JsonResult> FindLoginLogAnalysis() {
            return Json(await _loginLogLogic.FindLoginLogAnalysis());
        }
        /// <summary>
        ///     日志详情
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> LoginDetails(NullableIdInput input) {
            SystemLoginLog systemLogin = new SystemLoginLog();
            if (!input.Id.IsNullOrEmptyGuid())
            {
                systemLogin = await _loginLogLogic.GetById(input.Id);
            }
            return View(systemLogin);
        }
     
        #endregion

        #region 操作日志
        /// <summary>
        ///     操作日志
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("操作日志-列表")]
        public ActionResult OperationLog() {
            return View();
        }
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("操作日志-方法-列表-获取所有操作日志信息")]
        public async Task<JsonResult> GetPagingOperationLog(SystemLoginLogPagingInput queryParam) {
            return JsonForGridPaging(await _operationLogLogic.GetPagingOperationLog(queryParam));
        }
        /// <summary>
        ///     操作日志详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ActionResult> OperationDetails(NullableIdInput input) {
            SystemOperateLog operateLog = new SystemOperateLog();
            if (!input.Id.IsNullOrEmptyGuid())
            {
                operateLog = await _operationLogLogic.GetById(input.Id);
            }
            return View(operateLog);
        }
        #endregion

        #region 错误日志
        [CreateBy("冯辉")]
        [Description("错误日志-列表")]
        public ActionResult ExceptionLog() {
            return View();
        }

        [CreateBy("冯辉")]
        [Description("错误日志-方法-列表-获取所有异常日志信息")]
        public async Task<JsonResult> GetPagingExceptionLog(SystemLoginLogPagingInput queryParam)
        {
            return JsonForGridPaging(await _exceptionLogLogic.PagingExceptionLogQuery(queryParam));
        }
        /// <summary>
        ///     错误日志详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ActionResult> ExceptionDetails(NullableIdInput input) {
            SystemExceptionLog exceptionLog = new SystemExceptionLog();
            if (!input.Id.IsNullOrEmptyGuid())
            {
                exceptionLog = await _exceptionLogLogic.GetById(input.Id);
            }
            return View(exceptionLog);
        }
        #endregion


    }
}