using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Paging;

namespace Ctrl.Domain.Models.Dtos.Article
{
    /// <summary>
    ///     文章分页dto
    /// </summary>
    public class SystemArticlePagingInput : QueryParam, IInputDto {
        /// <summary>
        ///     标题
        /// </summary>
        public string Title { get; set; }
    }
}
