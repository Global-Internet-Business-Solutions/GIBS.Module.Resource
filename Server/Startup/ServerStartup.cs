using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using GIBS.Module.Resource.Repository;
using GIBS.Module.Resource.Services;

namespace GIBS.Module.Resource.Startup
{
    public class ServerStartup : IServerStartup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // not implemented
        }

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            // not implemented
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IResourceService, ServerResourceService>();
            services.AddTransient<IReservationService, ServerReservationService>();
            services.AddTransient<IResourceAvailabilityService, ServerResourceAvailabilityService>();
            services.AddTransient<ITwilioSmsService, TwilioSmsService>();
            services.AddTransient<ISchedulePdfService, SchedulePdfService>();

            services.AddTransient<IResourceRepository, ResourceRepository>();
            services.AddTransient<IReservationRepository, ReservationRepository>();
            services.AddTransient<IResourceAvailabilityRepository, ResourceAvailabilityRepository>();

            services.AddDbContextFactory<ResourceContext>(opt => { }, ServiceLifetime.Transient);
        }
    }
}
