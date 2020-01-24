#if NETCOREAPP
using Ctrl.Core.Core.Http;
using Ctrl.Core.Entities.Paging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ctrl.Core.PetaPoco
{
    public static class SqlMapperUtil {
        public static Database CreateDbBase () {
           var dbBase = HttpContexts.Current.RequestServices.GetRequiredService<PetaPocoClient>();
            return dbBase;
        }
        #region 映射
        /// <summary>
        ///     新增实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Task<object> Insert<T> (T t) where T : class {
            using (var db = CreateDbBase ()) {
                var result = db.Insert (t);
                return Task.FromResult(result);
            }
        }
        /// <summary>
        /// 批量增加
        /// </summary>
        /// <typeparam name="object"></typeparam>
        public static Task<int>InsertBatch<T>(IEnumerable<T> lt)where T:class{
            using(var db=CreateDbBase()){
                db.BulkInsert(lt);
                return Task.FromResult(1);
            }

        }

    
        /// <summary>
        ///     更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Task<int> Update<T> (T t) where T : class {
            using (var db = CreateDbBase ()) {
                var result = db.Update (t);
                return Task.FromResult(result);
            }
        }
        /// <summary>
        ///    更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">实体</param>
        /// <param name="pkey"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static Task<int> Update(string tableName,string pkey,object poco,IEnumerable<string> columns){
            using (var db=CreateDbBase())
            {
                var result = db.Update(tableName, pkey,poco,columns);
                return Task.FromResult(result);
            }
        }

        /// <summary>
        ///     删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Task<int> Delete<T> (T t) where T : class {
            using (var db = CreateDbBase ()) {
                var result = db.Delete (t);
                return Task.FromResult(result);
            }
        }
        /// <summary>
        ///     根据主键编码删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<int> DeleteById<T>(object id) {
            using (var db = CreateDbBase())
            {
       
               var result= db.Delete<T>(id);
                return Task.FromResult(result);
            }
        }
        /// <summary>
        ///     查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static Task<IEnumerable<T>> Query<T> (string sql = null, params object[] args) where T : class {
            using (var db = CreateDbBase ()) {
                var result = db.Query<T> (sql, args);
                return Task.FromResult(result);
            }
        }
        /// <summary>
        ///     查询满足条件的第一个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static Task<T> SingleOrDefault<T> (string sql = null) where T : class {
            using (var db = CreateDbBase ()) {
                var result = db.SingleOrDefault<T> (sql);
                return Task.FromResult(result);
            }

        }
        public static Task<T> FirstOrDefault<T> (string sql, params object[] args) where T : class {
            using (var db = CreateDbBase ()) {
                var result = db.FirstOrDefault<T> (sql, args);
                return Task.FromResult(result);
            }
        }
        /// <summary>
        ///     根据主键编码获取值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<T> GetById<T> (object id) {
            using(var db=CreateDbBase()){
                var result=db.SingleOrDefault<T>(id);
                return Task.FromResult(result);
            }
        }
        /// <summary>
        ///     是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<bool> Exists<T> (object id) {
            using (var db = CreateDbBase ()) {
                var result = db.Exists<T>(id);
                return Task.FromResult(result);
            }
        }
        /// <summary>
        ///     查询总数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Task<int> Count(string sql,params object[]args) {
            using (var db=CreateDbBase())
            {
                var result = db.ExecuteScalar<int>(sql,args);
                return Task.FromResult(result);
            }
        }
        /// <summary>
        ///     sql分页
        ///     在不影响的Pero中间件的情况下修改此分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<PagedResults<T>> PagingQuery<T> (string sql, QueryParam queryParam) {
            using (var db = CreateDbBase ()) {
                string OrderBy = !string.IsNullOrEmpty (queryParam.Sidx) ?
                    $" Order by {queryParam.Sidx} {queryParam.Sord}" :
                    "Order by (select 0)";
                sql += OrderBy;
                var result = db.Page<T> (queryParam.Pageindex, queryParam.Pagerow, sql);
                var pagerInfo = new PagerInfo ();
                pagerInfo.PageCount = result.TotalPages;
                pagerInfo.Pageindex = result.CurrentPage;
                pagerInfo.Pagerow = result.ItemsPerPage;
                pagerInfo.RecordCount = result.TotalItems;
                
                return Task.FromResult(new PagedResults<T> { Data = result.Items, pagerInfo = pagerInfo });
            }
        }

        #endregion

        #region 增删改
        public static Task<bool> InsertUpdateOrDeleteSqlBool(string sql,dynamic parms=null)
        {

            using(var db=CreateDbBase()){
                int result=db.Execute(sql,parms);
                return Task.FromResult(result>1);

            }
        }
        /// <summary>
        /// 执行语句返回bool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static Task<bool> SqlWithParamsBool<T>(string sql, dynamic parms = null)
        {
            using (var db = CreateDbBase())
            {
                var result = db.Query<T>(sql, (object)parms).Any();
                return Task.FromResult(result);
            }
        }
        /// <summary>
        ///     执行Sql语句带参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <param name="isSetConnectionStr"></param>
        /// <returns></returns>
        public static Task<IEnumerable<T>> SqlWithParams<T>(string sql, dynamic parms = null, bool isSetConnectionStr = true)
        {
            using (var db = CreateDbBase())
            {
                var result = db.Query<T>(sql, (object)parms);
                return Task.FromResult(result);
            }
        }
        #endregion
    }
}
#endif