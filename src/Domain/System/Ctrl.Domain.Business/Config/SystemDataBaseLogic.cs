using System.Collections.Generic;
using System.Threading.Tasks;
using Ctrl.Core.Business;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.DataAccess.Config;
using Ctrl.Domain.Models.Dtos.Config;

namespace Ctrl.Domain.Business.Config
{
    /// <summary>
    ///     系统配置文件接口实现
    /// </summary>
    public class SystemDataBaseLogic:AsyncLogic<SystemDataBaseTableOutput>,ISystemDataBaseLogic
    {
        #region 构造函数

        private readonly ISystemDataBaseRepository _dataBaseRepository;

        public SystemDataBaseLogic(ISystemDataBaseRepository dataBaseRepository)
            : base(dataBaseRepository)
        {
            _dataBaseRepository = dataBaseRepository;
        }
        #endregion


        #region 方法
        /// <summary>
        ///     获取所有表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SystemDataBaseTableOutput>> GetDataBaseTables()
        {
            return _dataBaseRepository.GetDataBaseTables();
        }


        /// <summary>
        ///   根据表名获取所有列名
        /// </summary>
        /// <param name="idInput"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemDataBaseColumnOutput>> GetDataBaseColumn(IdInput idInput)
        {
            return _dataBaseRepository.GetDataBaseColumn(idInput);
        }
        /// <summary>
        ///     根据表名获取对应的列明tree
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Task<IEnumerable<TreeEntity>> GetDataBaseColumnsTree(string Name) {
            return _dataBaseRepository.GetDataBaseColumnsTree(Name);
        }
        #endregion


    }
}
