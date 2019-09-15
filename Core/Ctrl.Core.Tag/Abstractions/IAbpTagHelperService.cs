using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.Core.Tag.Abstractions
{
    public interface IAbpTagHelperService<TTagHelper> where TTagHelper:TagHelper
    {
        TTagHelper TagHelper { get; }

        int Order { get; }

        void Init(TagHelperContext context);

        void Process(TagHelperContext context,TagHelperOutput output);

       
    }
}
