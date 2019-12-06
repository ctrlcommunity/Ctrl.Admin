using NLog;
using NLog.LayoutRenderers;
using System.Reflection;
using System.Text;

namespace Ctrl.Core.Core.Log.NLogLayout
{
    [LayoutRenderer("Convert")]
    public sealed class PropertyLayoutAppend : LayoutRenderer
    {
        /// <summary>
        ///     属性值
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        ///    重写基类转换器,根据配置的property key获取消息对象上的相应属性
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (Property != null)
                builder.Append(LookupProperty(Property, logEvent));
        }
        /// <summary>
        ///     通过反射获取传入的日志对象的某个属性的值
        /// </summary>
        /// <returns></returns>
        private static object LookupProperty(string property,LogEventInfo logEvent) {
            object messageObject = logEvent.Parameters[0];
            PropertyInfo propertyInfo = messageObject.GetType().GetProperty(property);
            object propertyValue = propertyInfo != null ? propertyInfo.GetValue(messageObject, null) : string.Empty;
            return propertyValue;
        }
    }
}
