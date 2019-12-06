using Microsoft.Extensions.Options;
using System;

namespace Ctrl.Core.PetaPoco
{
    public sealed class PetaPocoClient : Database
    {
        public PetaPocoClient(DbOptions options):base(options.ConnectionString,options.Name) { 
        
        }
    }
}
