using Ctrl.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.Core.Business
{
    /// <summary>
    ///     业务逻辑基类：异步
    /// </summary>
    /// <typeparam name="T">实体信息</typeparam>
    public interface IAsyncLogic<T> where T : class
    {
        /// <summary>
        ///     新增
        /// </summary>
        /// <param name="entity">新增实体</param>
        /// <returns></returns>
        Task<OperateStatus> InsertAsync(T entity);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="current">更新实体</param>
        /// <returns></returns>
        Task<OperateStatus> UpdateAsync(T current);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<OperateStatus> DeleteAsync(T entity);
        /// <summary>
        /// 获取集合数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllEnumerableAsync();
        ///<summary>
        /// 根据主键获取数据
        /// </summary>
        Task<T>GetById(object id);
    }
}
