using System.Threading.Tasks;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Tag.Controls.Button;
using Ctrl.Core.Tag.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ctrl.Core.Tag.Controls.Buttons
{
    [HtmlTargetElement("ctrl-button", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class CtrlButtonTagHelper: CtrlButtonTagHelperServiceBase
    {

        public CtrlButtonType ButtonType { get; set; }= CtrlButtonType.Primary;
        public CtrlButtonSize Size { get; set; } = CtrlButtonSize.Default;

        public string Text { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "button";
            NormalizeTagMode(context,output);
            AddClasses(context, output);
            AddText(context, output);
        }

        protected virtual void NormalizeTagMode(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
        }

        /// <summary>
        ///     添加class
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        protected virtual void AddClasses(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.AddClass("btn");

            if (ButtonType != CtrlButtonType.Default)
            {
                output.Attributes.AddClass("btn-" + ButtonType.ToString().ToLowerInvariant().Replace("_", "-"));
            }
            if (Size != CtrlButtonSize.Default)
            {
                output.Attributes.AddClass(Size.ToClassName());
            }
        }

        protected virtual void AddIcon()
        {

        }
        /// <summary>
        ///     添加文字
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        protected virtual void AddText(TagHelperContext context, TagHelperOutput output)
        {
            if (Text.IsNullOrWhiteSpace())
            {
                return;
            }
            output.Content.AppendHtml($"<span>{Text}</span>");
        }
        /// <summary>
        ///     添加禁用属性
        /// </summary>
        protected virtual void AddDisabled()
        {

        }

    }
}
