using Ctrl.Core.Entities.Dtos;
using Ctrl.System.Models.Entities;

namespace Ctrl.Domain.Models.Dtos.Identity
{
    /// <summary>
    ///     角色dto
    /// </summary>
    public class SystemRoleOutput:SystemRole,IOutputDto
    {
        /// <summary>
        ///     组织机构名称
        /// </summary>
        /// <value></value>
        public string OrganizationName { get; set; }
    }
}