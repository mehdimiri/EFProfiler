using EFProfiler.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
        private static EFProfilerSetting _efProfilerSetting { get; set; }
        public static IApplicationBuilder EFProfilerUI(this IApplicationBuilder app, IConfiguration configuration)
        {
            _efProfilerSetting = configuration.GetSection("EFProfilerSetting").Get<EFProfilerSetting>();
            return app.UseMiddleware<EFProfilerUIMiddleware>(_efProfilerSetting);
        }
    }
}
