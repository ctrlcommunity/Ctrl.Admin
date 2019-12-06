using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Core.PetaPoco
{
    public class DbOptions
    {
        public string Name { get; set; }

        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }
    }
}
