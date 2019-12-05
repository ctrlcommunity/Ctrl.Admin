using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;

namespace Ctrl.Core.Tag.Extensions
{
    public static class CtrlTagHelperAttributeListExtensions
    {
        public static void AddClass(this TagHelperAttributeList attributes, string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return;
            }

            var classAttribute = attributes["class"];
            if (classAttribute == null)
            {
                attributes.Add("class", className);
            }
            else
            {
                var existingClasses = classAttribute.Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                existingClasses.Add(className);
                attributes.SetAttribute("class", string.Join(" ", existingClasses));
            }
        }

        public static void RemoveClass(this TagHelperAttributeList attributes, string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return;
            }

            var classAttribute = attributes["class"];
            if (classAttribute == null)
            {
                return;
            }

            var classList = classAttribute.Value.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            classList.RemoveAll(c => c == className);

            attributes.SetAttribute("class", string.Join(" ",classList));
        }

        public static void AddIfNotContains(this TagHelperAttributeList attributes, string name, object value)
        {
            if (!attributes.ContainsName(name))
            {
                attributes.Add(name, value);
            }
        }
    }
}
