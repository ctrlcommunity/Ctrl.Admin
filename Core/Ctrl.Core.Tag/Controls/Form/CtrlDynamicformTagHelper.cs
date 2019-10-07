using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ctrl.Core.Tag.Controls.Form
{
    /// <summary>
    ///     动态form
    /// </summary>
    [HtmlTargetElement("ctrl-dynamic-form",TagStructure=TagStructure.NormalOrSelfClosing)]
    public class CtrlDynamicformTagHelper:CtrlTagHelperBase
    {
        [HtmlAttributeName("ctrl-model")]
        public ModelExpression Model { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

        }




    }
}
