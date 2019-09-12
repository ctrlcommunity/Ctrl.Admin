using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Article;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
	 /// <summary>
    /// 文章数据访问接口
    /// </summary>
    public interface ISystemArticleRepository: IRepository<SystemArticle>
    {
        /// <summary>
        ///     获取文章分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<PagedResults<SystemArticleOutput>> GetPagingArticleType(SystemArticlePagingInput param);
    }
}
