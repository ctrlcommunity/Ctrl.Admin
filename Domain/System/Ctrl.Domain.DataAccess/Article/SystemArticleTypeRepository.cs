using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Entities.Tree;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Article;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
    /// <summary>
    ///     文章类型数据访问接口实现
    /// </summary>
    public class SystemArticleTypeRepository : PetaPocoRepository<SystemArticleType>, ISystemArticleTypeRepository
    {
        /// <summary>
        ///     获取文章类型树
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<TreeEntity>> GetArticleTypeTree()
        {
            string sql = "select Name,ArticleTypeId id,ParentId pId from Sys_ArticleType";
            return SqlMapperUtil.Query<TreeEntity>(sql);
        }
        /// <summary>
        ///     获取文章类型分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemArticleTypeOutput>> GetPagingArticleType(QueryParam param)
        {
            string sql = @"select arttype.* ,arttype1.Name ParentName
            from Sys_ArticleType arttype
            left join Sys_ArticleType arttype1 on arttype.ParentId=arttype1.ArticleTypeId";
            return SqlMapperUtil.PagingQuery<SystemArticleTypeOutput>(sql, param);
        }


    }
}
