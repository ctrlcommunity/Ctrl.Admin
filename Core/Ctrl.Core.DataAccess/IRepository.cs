using Ctrl.Core.Entities.Paging;
using Ctrl.Core.PetaPoco;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.Core.DataAccess
{
    public interface IRepository<T>where T:class
    {
        /// <summary>
        ///     新增
        /// </summary>
        /// <param name="entity">新增实体</param>
        /// <returns></returns>
        Task<object> Insert(T entity);
        ///<summary>
        ///     PetaPoco批量新增
        /// </summary>
        Task<int>InsertMultiplePetaPocoAsync(IEnumerable<T> lt);
        /// <summary>
        ///   修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update(T entity);
        /// <summary>
        ///     更新
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pkey">主键key</param>
        /// <param name="poco">指定要更新的列值的POCO对象</param>
        /// <param name="columns">要更新的列的列名称，或全部为空</param>
        /// <returns></returns>
        Task<int> Update(string tableName, string pkey, object poco, IEnumerable<string> columns);
        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(T entity);
        /// <summary>
        ///     根据主键编码删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<int> DeleteById(object Id);
        /// <summary>
        ///     查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Query(string sql=null);
        /// <summary>
        ///     查询满足条件的第一个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<T> SingleOrDefault(string sql);
        ///<summary>
        ///     根据主键获取数据
        /// </summary>
        Task<T>GetById(object id);
        /// <summary>
        ///     查询满足条件的第一个数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<T> FirstOrDefault(string sql, params object[] args);

        /// <summary>
        ///     是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Exists(object id);

        Task<PagedResults<T>> PagingQuery(QueryParam queryParam, string sql);



    }
}
