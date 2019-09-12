using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.System.DataAccess;
using Ctrl.Domain.Models.Dtos;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos.Article;

namespace Ctrl.System.Business
{
    /// <summary>
    ///     文章业务逻辑接口实现
    /// </summary>
    public class SystemArticleLogic:AsyncLogic<SystemArticle>,ISystemArticleLogic
    {
        #region 构造函数
        private readonly ISystemArticleRepository _systemArticleRepository;

        public SystemArticleLogic(ISystemArticleRepository systemArticleRepository):base(systemArticleRepository) {
            _systemArticleRepository = systemArticleRepository;
        }
        #endregion

        #region 方法
        /// <summary>
        ///     保存文章
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public  Task<OperateStatus> SaveArticle(SystemArticle article)
        {
            if (article.ArticleId.IsEmptyGuid())
            {
                article.ArticleId = CombUtil.NewComb();
                return  InsertAsync(article);
            }
            else {
                return  UpdateAsync(article);
            }
        }
        /// <summary>
        ///     获取文章分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemArticleOutput>> GetPagingArticle(SystemArticlePagingInput param)
        {
            return _systemArticleRepository.GetPagingArticleType(param);
        }

        #endregion
    }
}