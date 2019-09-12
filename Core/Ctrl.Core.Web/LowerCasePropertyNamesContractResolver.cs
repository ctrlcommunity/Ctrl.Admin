using Newtonsoft.Json.Serialization;

namespace Ctrl.Core.Web
{
    /// <summary>
    ///     重写json命名
    /// </summary>
    public class LowerCasePropertyNamesContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
