using Ctrl.Core.Business;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Entities.Select2;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.Models.Dtos;
using Ctrl.Domain.Models.Dtos.Config;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.System.Business
{
    /// <summary>
    /// 字典业务逻辑接口
    /// </summary>
    public interface ISystemDictionaryLogic : IAsyncLogic<SystemDictionary>
    {

        /// <summary>
        ///     保存字典
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<OperateStatus> SaveSystemDictionary(SystemDictionary input);
        /// <summary>
        ///     获取字典树
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TreeEntity>> GetDictionaryTree();
        /// <summary>
        ///     字典分页信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<PagedResults<SystemDictionaryOutput>> PagingDictionaryQuery(SystemDictionaryPagingInput query);
        /// <summary>
        ///     根据上级编码获取子级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<Select2Entity>> GetTypeChildrenByCode(IdInput input);
    }
}
