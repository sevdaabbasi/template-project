using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Modules
{
    public static class ModuleExtensions
    {
        private static readonly List<IModule> DiscoveredModules = new();

        public static IServiceCollection AddModules(
            this IServiceCollection services, 
            IConfiguration configuration,
            params Assembly[] assemblies)
        {
            // Verilen assembly'ler içindeki IModule tipindeki sınıfları bul
            var modules = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IModule>();

            foreach (var module in modules)
            {
                module.Register(services, configuration);
                DiscoveredModules.Add(module);
            }

            return services;
        }

        public static IApplicationBuilder UseModules(this IApplicationBuilder app)
        {
            foreach (var module in DiscoveredModules)
            {
                module.UseModule(app);
            }

            return app;
        }
    }
}
