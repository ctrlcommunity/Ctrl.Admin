using System.Threading.Tasks;
using Ctrl.Core.Tag.Controls.Button;
using Ctrl.Core.Tag.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ctrl.Core.Tag.Controls.Buttons
{
    [HtmlTargetElement("ctrl-button", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class CtrlButtonTagHelper: CtrlButtonTagHelperServiceBase
    {

        public CtrlButtonType ButtonType { get; set; }= CtrlButtonType.Primary;

        public string Text { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            AddClasses(context,output);
            return base.ProcessAsync(context, output);
        }


        protected void AddClasses(TagHelperContext context,TagHelperOutput output) {
            output.Attributes.AddClass("btn");

        }

    }
}
