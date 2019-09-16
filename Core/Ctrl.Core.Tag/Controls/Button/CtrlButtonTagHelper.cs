using Ctrl.Core.Tag.Controls.Button;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ctrl.Core.Tag.Controls.Buttons
{
    [HtmlTargetElement("ctrl-button", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class CtrlButtonTagHelper:CtrlTagHelperBase
    {
        public CtrlButtonType ButtonType { get; set; }= CtrlButtonType.Default;

        public string BgColor { get; set; } = "primary";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "button";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", $"btn btn-{BgColor}");
            output.Attributes.SetAttribute("type", ButtonType.ToString());
            output.Content.SetContent(ButtonType.ToString() == "submit" ? "Add" : "Reset");
        }
    }
}
