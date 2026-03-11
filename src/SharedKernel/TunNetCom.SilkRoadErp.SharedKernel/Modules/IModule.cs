using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TunNetCom.SilkRoadErp.SharedKernel.Modules;

public interface IModule
{
    string Name { get; }
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}
