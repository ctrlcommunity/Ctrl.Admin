using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Ctrl.Core.Tag.Abstractions
{
    public interface ICtrlTagHelperService<TTagHelper> where TTagHelper:TagHelper
    {
        TTagHelper TagHelper { get; }

        int Order { get; }

        void Init(TagHelperContext context);

        void Process(TagHelperContext context,TagHelperOutput output);

        Task ProcessAsync(TagHelperContext context,TagHelperOutput output);
    }
}
