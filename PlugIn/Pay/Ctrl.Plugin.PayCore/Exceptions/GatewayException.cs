using System;

namespace Ctrl.Plugin.PayCore.Exceptions
{
    public class GatewayException : Exception
    {
        public GatewayException(string message)
            : base(message)
        {
        }
    }
}
