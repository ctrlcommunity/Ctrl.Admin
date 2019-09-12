using Ctrl.Core.Entities.Paging;
using Ctrl.Core.PetaPoco;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.Core.DataAccess
{
    /// <summary>
    ///     PetaPocoRepository仓储,T代表实体信息,规范约束为T必须继承IEntityBase接口
    /// </summary>
    /// <typeparam name="T">实体</typeparam>
    public class PetaPocoRepository<T> : BaseRepository, IRepository<T> where T : class, new()
    {
        public Task<int> Delete(T entity)
        {
            return SqlMapperUtil.Delete(entity);
        }
        /// <summary>
        ///     根据主键编码删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<int> DeleteById(object Id)
        {
            return SqlMapperUtil.DeleteById<T>(Id);
        }

        public Task<bool> Exists(object id)
        {
            return SqlMapperUtil.Exists<T>(id);
        }

        public Task<object> Insert(T entity)
        {
            return SqlMapperUtil.Insert(entity);
        }

        public Task<PagedResults<T>> PagingQuery(QueryParam queryParam, string sql)
        {
            return SqlMapperUtil.PagingQuery<T>(sql,queryParam);
        }
      
        public Task<IEnumerable<T>> Query(string sql=null)
        {
            return SqlMapperUtil.Query<T>(sql);
        }

        public Task<T> SingleOrDefault(string sql)
        {
            return SqlMapperUtil.SingleOrDefault<T>(sql);
        }
        public Task<T> FirstOrDefault(string sql, params object[] args)
        {
            return SqlMapperUtil.FirstOrDefault<T>(sql,args);
        }

        public Task<int> Update(T entity)
        {
            return SqlMapperUtil.Update(entity);
        }
        public Task<int> Update(string tableName, string pkey, object poco, IEnumerable<string> columns)
        {
            return SqlMapperUtil.Update(tableName,pkey,poco,columns);
        }

        public Task<T> GetById(object id)
        {
            return SqlMapperUtil.GetById<T>(id);
        }
        /// <summary>
        ///     批量新增
        /// </summary>
        /// <param name="lt"></param>
        /// <returns></returns>
        public Task<int> InsertMultiplePetaPocoAsync(IEnumerable<T> lt)
        {
            return SqlMapperUtil.InsertBatch(lt);
        }



    }
}
