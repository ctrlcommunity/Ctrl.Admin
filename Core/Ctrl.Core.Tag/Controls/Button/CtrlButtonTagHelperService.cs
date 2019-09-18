using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.Core.Tag.Controls.Button
{
    public  class CtrlButtonTagHelperService<TTagHelper>: TagHelper where TTagHelper:TagHelper
    {
        public CtrlButtonTagHelperService() {
          
        }
        public TTagHelper TagHelper { get; internal set; }

    }
}
