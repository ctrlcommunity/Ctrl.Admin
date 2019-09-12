using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.Models.Dtos.Article;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
    /// <summary>
    /// 文章类型数据访问接口
    /// </summary>
    public interface ISystemArticleTypeRepository: IRepository<SystemArticleType>
    {
        Task<IEnumerable<TreeEntity>> GetArticleTypeTree();
        /// <summary>
        ///     获取文章类型分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<PagedResults<SystemArticleTypeOutput>> GetPagingArticleType(QueryParam param);
    }
}
