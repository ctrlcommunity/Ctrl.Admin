using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Core.AutoMapper
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MapToTypeAttribute : Attribute
    {
        public MapToTypeAttribute(Type type) : this(type, false)
        {
        }

        public MapToTypeAttribute(Type type, bool reverseMap)
        {
            this.Type = type;
            this.ReverseMap = reverseMap;
        }

        public Type Type { get; private set; }
        public bool ReverseMap { get; set; } = false;
    }
}
