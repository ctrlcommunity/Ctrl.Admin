using Ctrl.Core.DataAccess;
using Ctrl.Core.PetaPoco;
using Ctrl.System.Models.Entities;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
    /// <summary>
    ///      支付配置表数据访问接口实现
    /// </summary>
    public class SystemPaysRepository : PetaPocoRepository<SystemPays>, ISystemPaysRepository
    {
        /// <summary>
        ///     获取支付方式信息
        /// </summary>
        /// <returns></returns>
        public Task<SystemPays> GetPaysInfoByType(string TypeName)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select top 1 pay.* from Sys_Pays pay
                        left join Sys_Dictionary dic on pay.PayType=dic.DictionaryId
                        where dic.Code=@Code  ");
            return SqlMapperUtil.FirstOrDefault<SystemPays>(sql.ToString(),new {Code=TypeName });
        }
    }
}
