using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Core.AutoMapper
{

    [AttributeUsage(AttributeTargets.Class)]
    public class MapFromTypeAttribute : Attribute
    {
        public MapFromTypeAttribute(Type type) : this(type, false)
        {
        }

        public MapFromTypeAttribute(Type type, bool reverseMap)
        {
            this.type = type;
        }

        public Type type { get; private set; }

        public bool ReverseMap { get; set; } = false;
    }
}
