using EFProfiler.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EFProfiler.EFProfilerUI
{
    public class EFProfilerUIOptions
    {
        public string RoutePrefix { get; set; } = "efprofiler";

        public Func<Stream> IndexStream { get; set; } = () => typeof(EFProfilerUIOptions).GetTypeInfo().Assembly
         .GetManifestResourceStream("EFProfiler.EFProfilerUI.index.html");

        public string DocumentTitle { get; set; } = "EFProfiler UI";

        public string HeadContent { get; set; } = "EFProfiler";

        public EFProfleAuthorization Authorization { get; set; }
    }
}
