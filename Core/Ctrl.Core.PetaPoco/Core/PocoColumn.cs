using System;
using System.Reflection;

namespace Ctrl.Core.PetaPoco.Core
{
    public class PocoColumn
    {
        public string ColumnName;
        public bool ForceToUtc;
        public PropertyInfo PropertyInfo;
        public bool ResultColumn;
        public bool AutoSelectedResultColumn;
        public string InsertTemplate { get; set; }
        public string UpdateTemplate { get; set; }

        public virtual void SetValue(object target, object val)
        {
            PropertyInfo.SetValue(target, val, null);
        }

        public virtual object GetValue(object target)
        {
            return PropertyInfo.GetValue(target, null);
        }

        public virtual object ChangeType(object val)
        {
            var t = PropertyInfo.PropertyType;
            if (val.GetType().IsValueType && PropertyInfo.PropertyType.IsGenericType && PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                t = t.GetGenericArguments()[0];

            return Convert.ChangeType(val, t);
        }
    }
}
