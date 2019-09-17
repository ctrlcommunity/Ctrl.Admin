using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Core.Tag.Controls.Button
{
    public interface IButtonTagHelperBase
    {
        CtrlButtonType ButtonType { get; }

      //  AbpButtonSize Size { get; }

        string Text { get; }

        string Icon { get; }

        bool? Disabled { get; }

    }
}
