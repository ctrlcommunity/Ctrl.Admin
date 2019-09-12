using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Web;
using Ctrl.Core.Web.Attributes;
using Ctrl.Domain.Models.Dtos.Article;
using Ctrl.System.Business;
using Ctrl.System.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Ctrl.Net.Areas.sysManage.Controllers
{
    /// <summary>
    /// 文章控制器
    /// </summary>
    public class  ArticleController : BaseController
    {
        #region 构造函数

        private readonly ISystemArticleLogic _systemArticleLogic;


        public ArticleController(ISystemArticleLogic systemArticleLogic)
        {
            _systemArticleLogic = systemArticleLogic;
        }

        #endregion

        #region 视图
		 /// <summary>
        ///     列表
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-文章-列表")]
        public IActionResult Index()
        {
            return View();
        }
		 /// <summary>
        ///     编辑
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-文章-编辑")]
        [Permission("xtgl-wz-SaveArticle")]
        public async Task<IActionResult> Edit(NullableIdInput input)
        {
            SystemArticle Article = new SystemArticle();
            if (!input.Id.IsNullOrEmptyGuid())
            {
                Article = await _systemArticleLogic.GetById(input.Id);
            }
            return View(Article);
        }
        #endregion

        #region 方法
		/// <summary>
        ///     获取文章
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-文章-方法-获取文章")]
        public async Task<JsonResult> GetList()
        {
            return Json(await _systemArticleLogic.GetAllEnumerableAsync());
        }

        /// <summary>
        ///     获取文章分页
        /// </summary>
        /// <returns></returns>
        [CreateBy("冯辉")]
        [Description("应用系统-文章-方法-获取文章列表")]
        public async Task<JsonResult> GetPagingArticle(SystemArticlePagingInput param)
        {
            return JsonForGridPaging(await _systemArticleLogic.GetPagingArticle(param));
        }


        /// <summary>
        ///     保存文章
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("冯辉")]
        [ValidateAntiForgeryToken]
        [Description("应用系统-文章类型-方法-保存文章类型")]
        [Permission("xtgl-wz-SaveArticle")]
        public async Task<JsonResult> SaveArticle(SystemArticle article) {
            return Json(await _systemArticleLogic.SaveArticle(article));
        }
		#endregion
	}
}