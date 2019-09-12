using Ctrl.System.Models.Entities;

namespace Ctrl.Domain.Models.Dtos.Permission
{
    public class SystemMenuOutput:SystemMenu
    {
        public string ParentName { get; set; }
    }
}