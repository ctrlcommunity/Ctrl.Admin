using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.Core.Tag.Controls.Button
{
    public abstract class CtrlButtonTagHelperService<TTagHelper>: TagHelper where TTagHelper:TagHelper
    {
        public TTagHelper TagHelper { get; internal set; }

        public virtual int Order { get; }

        public virtual void Init(TagHelperContext context)
        {

        }

        public virtual void Process(TagHelperContext context, TagHelperOutput output)
        {

        }

        public virtual Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            Process(context, output);
            return Task.CompletedTask;
        }
    }
}
