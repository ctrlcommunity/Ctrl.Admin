using Ctrl.Core.Core.Resource;
using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.Core.Business
{
    /// <summary>
    ///     异步logic
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncLogic<T> : IAsyncLogic<T> where T : class, new()
    {
        public AsyncLogic() { }
        public IRepository<T> Repository;
        public AsyncLogic(IRepository<T> repository)
        {
            Repository = repository ?? throw new ArgumentNullException("repository", "repository为空");
            Repository = repository;
        }
        public async Task<OperateStatus> DeleteAsync(T entity)
        {
            var operateStatus = new OperateStatus();
            try
            {
                var resultNum = await Repository.Delete(entity);
                operateStatus.ResultSign =Convert.ToInt32(resultNum) > 0 ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = Convert.ToInt32(resultNum) > 0 ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }
        /// <summary>
        ///     根据主键编码删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<OperateStatus> DeleteById(object Id) {
            var operateStatus = new OperateStatus();
            try
            {
                var resultNum = await Repository.DeleteById(Id);
                operateStatus.ResultSign = Convert.ToInt32(resultNum) > 0 ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = Convert.ToInt32(resultNum) > 0 ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }

        public async Task<OperateStatus> InsertAsync(T entity)
        {
            var operateStatus = new OperateStatus();
            try
            {
                var resultNum = await Repository.Insert(entity);
                operateStatus.ResultSign = !(resultNum==null) ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = !(resultNum == null) ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }

        public async Task<OperateStatus> UpdateAsync(T current)
        {
            var operateStatus = new OperateStatus();
            try
            {
                var resultNum = await Repository.Update(current);
        
                operateStatus.ResultSign = Convert.ToInt32(resultNum) > 0 ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = Convert.ToInt32(resultNum) > 0 ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }
        /// <summary>
        ///     更新
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pkey">主键key</param>
        /// <param name="poco">指定要更新的列值的POCO对象</param>
        /// <param name="columns">要更新的列的列名称，或全部为空</param>
        /// <returns></returns>
        public async Task<OperateStatus> UpdateAsync(string tableName, string pkey, object poco, IEnumerable<string> columns)
        {
            var operateStatus = new OperateStatus();
            try
            {
                var resultNum = await Repository.Update(tableName,pkey,poco,columns);

                operateStatus.ResultSign = Convert.ToInt32(resultNum) > 0 ? ResultSign.Successful : ResultSign.Error;
                operateStatus.Message = Convert.ToInt32(resultNum) > 0 ? Chs.Successful : Chs.Error;
            }
            catch (Exception exception)
            {
                operateStatus.Message = string.Format(Chs.Error, exception.Message);
            }
            return operateStatus;
        }


        /// <summary>
        ///     获取集合数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<T>> GetAllEnumerableAsync()
        {
               return  Repository.Query();
        }
        /// <summary>
        /// 根据主键获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T> GetById(object id)
        {
            return  Repository.GetById(id);
        }
    }
}
