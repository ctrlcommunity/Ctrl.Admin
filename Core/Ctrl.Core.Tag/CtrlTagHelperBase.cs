using Ctrl.Core.Tag.Controls;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Ctrl.Core.Tag
{
    /// <summary>
    ///     taghelper基类
    /// </summary>
    public abstract  class CtrlTagHelperBase:TagHelper
    {


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
        }
      


    }
}