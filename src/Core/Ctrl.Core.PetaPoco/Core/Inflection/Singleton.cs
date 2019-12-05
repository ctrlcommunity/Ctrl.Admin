using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Core.PetaPoco.Core.Inflection
{
    internal static class Singleton<T> where T : new()
    {
        public static T Instance = new T();
    }
}
