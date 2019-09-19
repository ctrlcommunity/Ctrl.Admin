using System.Threading.Tasks;
using Ctrl.Core.Tag.Controls.Button;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ctrl.Core.Tag.Controls.Buttons
{
    [HtmlTargetElement("ctrl-button", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class CtrlButtonTagHelper: CtrlButtonTagHelperServiceBase
    {

        public CtrlButtonType ButtonType { get; set; }= CtrlButtonType.Primary;

        public string Text { get; set; }


       

        protected void AddClasses(TagHelperContext context,TagHelperOutput output) {
           // output.Attributes.
        }

    }
}
