using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Modules
{

    public interface IModule
    {

        void Register(IServiceCollection services, IConfiguration configuration);


        void UseModule(IApplicationBuilder app);
    }
}
