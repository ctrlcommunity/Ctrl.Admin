using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Ctrl.Core.Core.Attributes
{
    /// <summary>
    ///     表示一个特性,标识该特性的Action方法绕过认证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method,AllowMultiple =false,Inherited =true)]
    public class IgnoreAttribute:Attribute, IFilterMetadata
    {
    }
}
