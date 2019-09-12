using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Article;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.System.Business
{
    /// <summary>
    /// 文章类型业务逻辑接口
    /// </summary>
    public interface ISystemArticleTypeLogic: IAsyncLogic<SystemArticleType>
    {
        /// <summary>
        ///     保存文章类型
        /// </summary>
        /// <param name="articleType"></param>
        /// <returns></returns>
        Task<OperateStatus> SaveArticleType(SystemArticleType articleType);
        /// <summary>
        ///     获取文章类型树
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TreeEntity>> GetArticleTypeTree();
        /// <summary>
        ///     获取文章类型分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<PagedResults<SystemArticleTypeOutput>> GetPagingArticleType(QueryParam param);
    }
}
