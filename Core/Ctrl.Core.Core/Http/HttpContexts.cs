using Microsoft.AspNetCore.Http;

namespace Ctrl.Core.Core.Http
{
    public static class HttpContexts
    {
        private static IHttpContextAccessor _accessor;

        public static HttpContext Current => _accessor.HttpContext;

        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }
}
