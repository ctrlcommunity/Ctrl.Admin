using Ctrl.Core.Entities.Dtos;
using Ctrl.System.Models.Entities;

namespace Ctrl.Domain.Models.Dtos.Config
{
    /// <summary>
    ///     字典输出类
    /// </summary>
    public class SystemDictionaryOutput: SystemDictionary,IOutputDto
    {
        /// <summary>
        ///     父级名称
        /// </summary>
        public string ParentName { get; set; }
    }
}
