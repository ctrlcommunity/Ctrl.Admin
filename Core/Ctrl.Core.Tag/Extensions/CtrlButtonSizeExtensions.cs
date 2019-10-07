using Ctrl.Core.Tag.Controls.Button;

namespace Ctrl.Core.Tag.Extensions
{
    public static class CtrlButtonSizeExtensions
    {
        public static string ToClassName(this CtrlButtonSize size)
        {
            switch (size)
            {
                case CtrlButtonSize.Small:
                    return "btn-sm";
                case CtrlButtonSize.Medium:
                    return "btn-md";
                case CtrlButtonSize.Large:
                    return "btn-lg";
                case CtrlButtonSize.Block:
                    return "btn-block";
                case CtrlButtonSize.Block_Small:
                    return "btn-sm  btn-block";
                case CtrlButtonSize.Block_Medium:
                    return "btn-md  btn-block";
                case CtrlButtonSize.Block_Large:
                    return "btn-lg  btn-block";
                default:
                    return "";

            }
        }
    }
}
