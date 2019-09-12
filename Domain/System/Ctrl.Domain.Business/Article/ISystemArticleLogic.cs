using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Article;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.System.Business
{
    /// <summary>
    /// 文章业务逻辑接口
    /// </summary>
    public interface ISystemArticleLogic: IAsyncLogic<SystemArticle>
    {
        /// <summary>
        ///     保存文章
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        Task<OperateStatus> SaveArticle(SystemArticle article);
        /// <summary>
        ///     获取文章分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<PagedResults<SystemArticleOutput>> GetPagingArticle(SystemArticlePagingInput param);
    }
}
