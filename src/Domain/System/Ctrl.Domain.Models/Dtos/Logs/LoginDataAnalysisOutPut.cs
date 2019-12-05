using Ctrl.Core.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Domain.Models.Dtos.Logs
{
    /// <summary>
    ///     折线图数据
    /// </summary>
    public class LoginDataAnalysisOutPut:IOutputDto
    {
        /// <summary>
        ///    x 数据
        /// </summary>
        public List<string> xdata { get; set; }
        /// <summary>
        ///     y数据
        /// </summary>
        public List<int> ydata { get; set; }
    }
}
