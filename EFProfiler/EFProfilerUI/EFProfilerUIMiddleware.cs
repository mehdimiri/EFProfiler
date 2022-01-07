using EFProfiler.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#if NETSTANDARD2_0
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace EFProfiler.EFProfilerUI
{
    public class EFProfilerUIMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EFProfilerUIOptions _options;
        private readonly StaticFileMiddleware _staticFileMiddleware;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public EFProfilerUIMiddleware(
          [NotNull] RequestDelegate next,
          IWebHostEnvironment hostingEnv,
          ILoggerFactory loggerFactory,
          EFProfilerUIOptions options)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            _next = next;
            _options = options ?? new EFProfilerUIOptions();
            _staticFileMiddleware = CreateStaticFileMiddleware(_next, hostingEnv, loggerFactory, options);
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.IgnoreNullValues = true;
            _jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;
            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/?$", RegexOptions.IgnoreCase))
            {
                var relativeIndexUrl = string.IsNullOrEmpty(path) || path.EndsWith("/")
                    ? "index.html"
                    : $"{path.Split('/').Last()}/index.html";
                RespondWithRedirect(httpContext.Response, relativeIndexUrl);
                return;
            }

            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?index.html$", RegexOptions.IgnoreCase))
            {
                await RespondWithIndexHtml(httpContext);
                return;
            }

            await _staticFileMiddleware.Invoke(httpContext);
        }

        private StaticFileMiddleware CreateStaticFileMiddleware(
           RequestDelegate next,
           IWebHostEnvironment hostingEnv,
           ILoggerFactory loggerFactory,
           EFProfilerUIOptions options)
        {
            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = string.IsNullOrEmpty(options.RoutePrefix) ? string.Empty : $"/{options.RoutePrefix}",
                FileProvider = new EmbeddedFileProvider(typeof(EFProfilerUIMiddleware).GetTypeInfo().Assembly, ""),
            };

            return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
        }

        private void RespondWithRedirect(HttpResponse response, string location)
        {
            response.StatusCode = 301;
            response.Headers["Location"] = location;
        }

        private async Task RespondWithIndexHtml(HttpContext httpContext)
        {
            if (!string.IsNullOrEmpty(_options.Authorization.Roles) || !string.IsNullOrEmpty(_options.Authorization.Users))
            {
                if (_options.Authorization.Authorize(httpContext))
                {
                    await RenderDashboard(httpContext);
                }
                else
                {
                    await RenderInaccessibility(httpContext);
                }
                
            }
            else
            {
                await RenderDashboard(httpContext);
            }
        }

        private IDictionary<string, string> GetIndexArguments()
        {
            return new Dictionary<string, string>()
            {
                { "%(DocumentTitle)", _options.DocumentTitle },
                { "%(HeadContent)", _options.HeadContent },
                { "$LogItems$", JsonSerializer.Serialize(GetLogs(_options.FolderPathRepository), _jsonSerializerOptions) }
            };
        }

        private List<LogModel> GetLogs(string folderPath)
        {
            List<LogModel> lst = new List<LogModel>();
            if (Directory.Exists(folderPath))
            {
                foreach (var item in Directory.GetFiles(folderPath))
                {
                    lst.Add(JsonSerializer.Deserialize<LogModel>(File.ReadAllText(item)));
                }
            }
            return lst;
        }

        private async Task RenderDashboard(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "text/html;charset=utf-8";
            using (var stream = _options.IndexStream())
            {
                var htmlBuilder = new StringBuilder(new StreamReader(stream).ReadToEnd());
                foreach (var entry in GetIndexArguments())
                {
                    htmlBuilder.Replace(entry.Key, entry.Value);
                }

                await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
            }
        }

        private async Task RenderInaccessibility(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.ContentType = "text/html;charset=utf-8";
            using (var stream = _options.IndexStream())
            {
                var htmlBuilder = new StringBuilder(new StreamReader(stream).ReadToEnd());
                htmlBuilder.Replace("%(DocumentTitle)", _options.DocumentTitle);
                htmlBuilder.Replace("%(HeadContent)", "Lack of access to the dashboard");
                await httpContext.Response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
            }
        }
    }
}