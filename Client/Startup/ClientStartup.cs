using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Oqtane.Services;
using GIBS.Module.Resource.Services;

namespace GIBS.Module.Resource.Startup
{
    public class ClientStartup : IClientStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            if (!services.Any(s => s.ServiceType == typeof(IResourceService)))
            {
                services.AddScoped<IResourceService, ClientResourceService>();
            }

            if (!services.Any(s => s.ServiceType == typeof(IReservationService)))
            {
                services.AddScoped<IReservationService, ClientReservationService>();
            }

            if (!services.Any(s => s.ServiceType == typeof(IResourceAvailabilityService)))
            {
                services.AddScoped<IResourceAvailabilityService, ClientResourceAvailabilityService>();
            }
        }
    }
}
