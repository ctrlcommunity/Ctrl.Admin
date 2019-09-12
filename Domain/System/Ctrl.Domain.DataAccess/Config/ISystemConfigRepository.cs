using Ctrl.Core.DataAccess;
using Ctrl.Domain.Models.Dtos;
using Ctrl.System.Models.Entities;
using System.Threading.Tasks;

namespace Ctrl.System.DataAccess
{
	 /// <summary>
    /// 网站配置数据访问接口
    /// </summary>
    public interface ISystemConfigRepository: IRepository<SystemConfig>
    {
       
    }
}
