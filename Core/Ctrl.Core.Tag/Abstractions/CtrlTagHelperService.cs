using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.Core.Tag.Abstractions
{
    public abstract class CtrlTagHelperService<TTagHelper> : ICtrlTagHelperService<TTagHelper>
        where TTagHelper : TagHelper
    {
        protected const string FormGroupContents = "FormGroupContents";
        protected const string TabItems = "TabItems";
        protected const string AccordionItems = "AccordionItems";
        protected const string BreadcrumbItemsContent = "BreadcrumbItemsContent";
        protected const string CarouselItemsContent = "CarouselItemsContent";
        protected const string TabItemsDataTogglePlaceHolder = "{_data_toggle_Placeholder_}";
        protected const string TabItemNamePlaceHolder = "{_Tab_Tag_Name_Placeholder_}";
        protected const string CtrlFormContentPlaceHolder = "{_CtrlFormContentPlaceHolder_}";
        protected const string CtrlTabItemActivePlaceholder = "{_Tab_Active_Placeholder_}";
        protected const string CtrlTabDropdownItemsActivePlaceholder = "{_Tab_DropDown_Items_Placeholder_}";
        protected const string CtrlTabItemShowActivePlaceholder = "{_Tab_Show_Active_Placeholder_}";
        protected const string CtrlBreadcrumbItemActivePlaceholder = "{_Breadcrumb_Active_Placeholder_}";
        protected const string CtrlCarouselItemActivePlaceholder = "{_CarouselItem_Active_Placeholder_}";
        protected const string CtrlTabItemSelectedPlaceholder = "{_Tab_Selected_Placeholder_}";
        protected const string CtrlAccordionParentIdPlaceholder = "{_Parent_Accordion_Id_}";


        public TTagHelper TagHelper { get; internal set; }

        public int Order => throw new NotImplementedException();

        public void Init(TagHelperContext context)
        {
            throw new NotImplementedException();
        }

        public void Process(TagHelperContext context, TagHelperOutput output)
        {
            throw new NotImplementedException();
        }

        public Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            throw new NotImplementedException();
        }
    }
}
