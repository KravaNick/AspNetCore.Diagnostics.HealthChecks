using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*
             * If you have different hosted services, please check this open bug on 2.2 HealthChecks
             * https://github.com/aspnet/Extensions/issues/639 and the workaround proposed by @NatMarchand
             * or register all hosted service before call AddHealthChecks.
             */

            services
                .AddApplicationInsightsTelemetry()
                .AddHealthChecks()
                .AddNpgSql("Server=localhost1", name: "npg1")
                .AddNpgSql("Server=localhost2", name: "npg2");

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .UseHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true
                })
                .UseHealthChecks("/healthz", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                })
                .UseRouting()
                .UseEndpoints(config => config.MapDefaultControllerRoute());
        }

        public class RandomHealthCheck : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                if (DateTime.UtcNow.Minute % 2 == 0)
                {
                    return Task.FromResult(HealthCheckResult.Healthy());
                }

                return Task.FromResult(HealthCheckResult.Unhealthy(description: "failed", exception: new InvalidCastException("Invalid cast from to to to")));
            }

        }
    }
}
