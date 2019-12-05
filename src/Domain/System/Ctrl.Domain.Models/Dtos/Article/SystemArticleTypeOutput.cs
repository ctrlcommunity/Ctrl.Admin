using Ctrl.Core.Entities.Dtos;
using Ctrl.System.Models.Entities;

namespace Ctrl.Domain.Models.Dtos.Article
{
    /// <summary>
    ///     文章类型输出类
    /// </summary>
    public class SystemArticleTypeOutput: SystemArticleType,IOutputDto
    {
        public string ParentName { get; set; }
    }
}
