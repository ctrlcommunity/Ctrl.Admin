using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ctrl.Core.PetaPoco.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPetaPoco(this IServiceCollection services,Action<DbOptions> action) {
            DbOptions dbOptions = new DbOptions();
            action.Invoke(dbOptions);
            services.AddTransient(s => new PetaPocoClient(dbOptions));
            return services;
        }
    }
}
