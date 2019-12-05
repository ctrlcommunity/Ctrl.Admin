using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;
using Ctrl.Core.Entities.Select2;
using Ctrl.Core.Entities.Tree;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos.Config;
using Ctrl.System.Models.Entities;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
    /// <summary>
    ///     字典数据访问接口实现
    /// </summary>
    public class SystemDictionaryRepository : PetaPocoRepository<SystemDictionary>, ISystemDictionaryRepository
    {
        /// <summary>
        ///     获取字典树
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<TreeEntity>> GetDictionaryTree()
        {
            const string sql = "select Name,DictionaryId id,ParentId pId from Sys_Dictionary";
            return SqlMapperUtil.Query<TreeEntity>(sql);
        }
        /// <summary>
        ///     根据父级编码获取子级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<IEnumerable<Select2Entity>> GetTypeChildrenByCode(IdInput input)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select sds.DictionaryId id,sds.Name text  from Sys_Dictionary sd
                            left join Sys_Dictionary sds on sds.ParentId=sd.DictionaryId
                            where sd.Code='{0}'
                            order by sds.OrderNo desc", input.Id);
            return SqlMapperUtil.Query<Select2Entity>(sb.ToString());
        }

        /// <summary>
        ///     字典分页信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemDictionaryOutput>> PagingDictionaryQuery(SystemDictionaryPagingInput query)
        {
            string swhere = "";
            if (!string.IsNullOrWhiteSpace(query.Id))
            {
                swhere += $" And dic.ParentId='{query.Id}'";
            }
            string sql = $@" select dic.*,dic1.Name ParentName
                            from Sys_Dictionary dic
                            left join Sys_Dictionary dic1 on dic.ParentId=dic1.DictionaryId where 1=1 {swhere}";
            return SqlMapperUtil.PagingQuery<SystemDictionaryOutput>(sql,query);
        }
    }
}
