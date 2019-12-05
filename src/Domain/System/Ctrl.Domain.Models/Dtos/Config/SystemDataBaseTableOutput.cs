using Ctrl.Core.Entities.Dtos;
using System;

namespace Ctrl.Domain.Models.Dtos.Config
{
    public class SystemDataBaseTableOutput : IOutputDto
    {
        /// <summary>
        ///     表
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     表注释
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
