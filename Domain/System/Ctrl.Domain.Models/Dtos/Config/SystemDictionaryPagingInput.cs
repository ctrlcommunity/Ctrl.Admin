using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;

namespace Ctrl.Domain.Models.Dtos.Config
{
    /// <summary>
    ///     字典分页dto
    /// </summary>
    public class SystemDictionaryPagingInput : QueryParam, IInputDto
    {
        public string Id { get; set; }
    }
}
