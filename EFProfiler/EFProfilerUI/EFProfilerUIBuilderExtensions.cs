using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFProfiler.EFProfilerUI
{
    public static class EFProfilerUIBuilderExtensions
    {
        public static IApplicationBuilder EFProfilerUI(this IApplicationBuilder app, EFProfilerUIOptions options)
        {
            return app.UseMiddleware<EFProfilerUIMiddleware>(options);
        }

        public static IApplicationBuilder EFProfilerUI(
         this IApplicationBuilder app,
         Action<EFProfilerUIOptions> setupAction = null)
        {
            EFProfilerUIOptions options;
            using (var scope = app.ApplicationServices.CreateScope())
            {
                options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<EFProfilerUIOptions>>().Value;
                setupAction?.Invoke(options);
            }

            return app.EFProfilerUI(options);
        }
    }
}
