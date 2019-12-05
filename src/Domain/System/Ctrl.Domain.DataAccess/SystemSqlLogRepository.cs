using Ctrl.Core.DataAccess;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
    /// <summary>
    ///     sql执行日志表数据访问接口实现
    /// </summary>
    public class SystemSqlLogRepository : PetaPocoRepository<SystemSqlLog>, ISystemSqlLogRepository
    {
      
    }
}
