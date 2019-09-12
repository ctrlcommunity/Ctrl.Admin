using Ctrl.Core.Entities.Dtos;
using Ctrl.Domain.Models.Enums;

namespace Ctrl.Domain.Models.Dtos.Permission
{
    public class GetPermissionByPrivilegeMasterValueInput:IInputDto
    {
        public string PrivilegeMasterValue { get; set; } 

        public EnumPrivilegeMaster PrivilegeMaster { get; set; }

        public EnumPrivilegeAccess PrivilegeAccess { get; set; }

        public string PrivilegeMenuId { get; set; }
    }
}