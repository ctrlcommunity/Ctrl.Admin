using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Article;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
    /// <summary>
    ///     文章数据访问接口实现
    /// </summary>
    public class SystemArticleRepository : PetaPocoRepository<SystemArticle>, ISystemArticleRepository
    {
        /// <summary>
        ///     获取文章分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemArticleOutput>> GetPagingArticleType(SystemArticlePagingInput param)
        {
            string sWhere = "";
            if (!string.IsNullOrWhiteSpace(param.Title))
            {
                sWhere += $" And Title like '%{param.Title.Trim()}%'";
            }
            string sql = @"select article.*,articletype.Name ArticleTypeName
                            from Sys_Article article
                            left join Sys_ArticleType articletype on article.ArticleTypeId=articletype.ArticleTypeId
                              Where 1=1 "+sWhere;
            return SqlMapperUtil.PagingQuery<SystemArticleOutput>(sql, param); ;
        }
    }
}
