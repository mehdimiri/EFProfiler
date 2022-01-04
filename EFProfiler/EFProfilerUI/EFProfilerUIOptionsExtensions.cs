using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFProfiler.EFProfilerUI
{
    public static class EFProfilerUIOptionsExtensions
    {
        public static void InjectStylesheet(this EFProfilerUIOptions options, string path, string media = "screen")
        {
            var builder = new StringBuilder(options.HeadContent);
            builder.AppendLine($"<link href='{path}' rel='stylesheet' media='{media}' type='text/css' />");
            options.HeadContent = builder.ToString();
        }

        public static void InjectJavascript(this EFProfilerUIOptions options, string path, string type = "text/javascript")
        {
            var builder = new StringBuilder(options.HeadContent);
            builder.AppendLine($"<script src='{path}' type='{type}'></script>");
            options.HeadContent = builder.ToString();
        }

    }
}
