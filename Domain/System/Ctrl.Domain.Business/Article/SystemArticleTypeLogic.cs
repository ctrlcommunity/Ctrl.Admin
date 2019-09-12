using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.System.DataAccess;
using Ctrl.Domain.Models.Dtos;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;
using Ctrl.Core.Core.Utils;
using System;
using Ctrl.Core.Entities.Tree;
using System.Collections.Generic;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.Models.Dtos.Article;

namespace Ctrl.System.Business
{
    /// <summary>
    ///     文章类型业务逻辑接口实现
    /// </summary>
    public class SystemArticleTypeLogic:AsyncLogic<SystemArticleType>,ISystemArticleTypeLogic
    {
        #region 构造函数
        private readonly ISystemArticleTypeRepository _systemArticleTypeRepository;

        public SystemArticleTypeLogic(ISystemArticleTypeRepository systemArticleTypeRepository):base(systemArticleTypeRepository) {
            _systemArticleTypeRepository = systemArticleTypeRepository;
        }



        #endregion

        #region 方法
        /// <summary>
        ///     保存文章类型
        /// </summary>
        /// <param name="articleType"></param>
        /// <returns></returns>

        public async Task<OperateStatus> SaveArticleType(SystemArticleType articleType)
        {
            if (articleType.ArticleTypeId.IsEmptyGuid())
            {
                articleType.CreateTime = DateTime.Now;
                articleType.ArticleTypeId = CombUtil.NewComb();
                return await InsertAsync(articleType);
            }
            else {
                var artType = await _systemArticleTypeRepository.GetById(articleType.ArticleTypeId);
                articleType.CreateTime = artType.CreateTime;
                return await UpdateAsync(articleType);
            }
        }
        /// <summary>
        ///     获取文章类型树
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<TreeEntity>> GetArticleTypeTree()
        {
            return _systemArticleTypeRepository.GetArticleTypeTree();
        }
        
        /// <summary>
        ///     获取文章类型分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemArticleTypeOutput>> GetPagingArticleType(QueryParam param)
        {
            return _systemArticleTypeRepository.GetPagingArticleType(param);
        }
        #endregion
    }
}