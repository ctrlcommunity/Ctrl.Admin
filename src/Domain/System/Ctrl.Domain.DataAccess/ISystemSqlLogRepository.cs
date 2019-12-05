using Ctrl.Core.DataAccess;
using Ctrl.Domain.Models.Dtos;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
	 /// <summary>
    /// sql执行日志表数据访问接口
    /// </summary>
    public interface ISystemSqlLogRepository: IRepository<SystemSqlLog>
    {
       
    }
}
