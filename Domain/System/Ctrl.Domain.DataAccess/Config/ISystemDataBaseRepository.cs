using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.Models.Dtos.Config;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ctrl.Domain.DataAccess.Config
{
    public interface ISystemDataBaseRepository:IRepository<SystemDataBaseTableOutput>
    {
        /// <summary>
        ///     获取所有表
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SystemDataBaseTableOutput>> GetDataBaseTables();
        /// <summary>
        ///     根据表名获取所有列
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SystemDataBaseColumnOutput>>GetDataBaseColumn(IdInput idInput);
        /// <summary>
        ///     根据表名获取所有列
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        Task<IEnumerable<TreeEntity>> GetDataBaseColumnsTree(string Name);
    }
}
