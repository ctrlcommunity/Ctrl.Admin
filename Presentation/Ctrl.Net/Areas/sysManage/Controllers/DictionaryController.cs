using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Web;
using Ctrl.Core.Web.Attributes;
using Ctrl.Domain.Models.Dtos.Config;
using Ctrl.System.Business;
using Ctrl.System.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities.Dtos;

namespace Ctrl.Net.Areas.sysManage.Controllers
{
    /// <summary>
    /// 字典控制器
    /// </summary>
    public class DictionaryController : BaseController
    {
        #region 构造函数

        private readonly ISystemDictionaryLogic _systemDictionaryLogic;


        public DictionaryController(ISystemDictionaryLogic systemDictionaryLogic)
        {
            _systemDictionaryLogic = systemDictionaryLogic;
        }

        #endregion

        #region 视图
        /// <summary>
        ///     列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-字典-列表")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///     编辑
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-字典-编辑")]
        [Permission("xtgl-zdgl-SaveSystemDictionary")]
        public async Task<IActionResult> Edit(NullableIdInput input)
        {
            SystemDictionary dictionary = new SystemDictionary();
            if (!input.Id.IsNullOrEmptyGuid())
            {
                dictionary = await _systemDictionaryLogic.GetById(input.Id);
            }
            return View(dictionary);
        }
        #endregion

        #region 方法
        /// <summary>
        ///     获取字典
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-字典-方法-获取字典")]
        public async Task<JsonResult> GetList()
        {
            return Json(await _systemDictionaryLogic.GetAllEnumerableAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("应用系统-方法-保存字典")]
        [Permission("xtgl-zdgl-SaveSystemDictionary")]
        public async Task<JsonResult> SaveSystemDictionary(SystemDictionary input)
        {
            return Json(await _systemDictionaryLogic.SaveSystemDictionary(input));
        }
        [CreateBy("冯辉")]
        [Description("应用系统-文章类型-方法-获取字典树")]
        public async Task<JsonResult> GetDictionaryTree()
        {
            return Json(await _systemDictionaryLogic.GetDictionaryTree());
        }

        [HttpPost]
        [CreateBy("冯辉")]
        [Description("字典管理-方法-列表-获取字典分页信息")]
        public async Task<JsonResult> GetPagingDictionary(SystemDictionaryPagingInput queryParam)
        {
            return JsonForGridPaging(await _systemDictionaryLogic.PagingDictionaryQuery(queryParam));
        }
        [HttpGet]
        [CreateBy("冯辉")]
        [Description("字典管理-方法-列表-获取子级类型")]
        public async Task<JsonResult> GetTypeChildrenByCode(IdInput input) {
            return Json(await _systemDictionaryLogic.GetTypeChildrenByCode(input));
        }

        #endregion
    }
}