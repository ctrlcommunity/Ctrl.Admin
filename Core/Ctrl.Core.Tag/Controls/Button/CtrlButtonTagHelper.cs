using System.Threading.Tasks;
using Ctrl.Core.Tag.Controls.Button;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ctrl.Core.Tag.Controls.Buttons
{
    [HtmlTargetElement("ctrl-button", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class CtrlButtonTagHelper: CtrlButtonTagHelperServiceBase<CtrlButtonTagHelper>,IButtonTagHelperBase
    {
        public CtrlButtonType ButtonType { get; set; }= CtrlButtonType.Primary;

        public string Text => throw new System.NotImplementedException();

        public string Icon => throw new System.NotImplementedException();

        public bool? Disabled => throw new System.NotImplementedException();
    
      


    }
}
