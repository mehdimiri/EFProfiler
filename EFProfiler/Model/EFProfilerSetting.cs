using EFProfiler.EFProfilerUI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFProfiler.Model
{
    [ExcludeFromCodeCoverage]
    public class EFProfilerSetting
    {
        public int MaxMillisecond { get; set; } = 0;
        public string Path { get; set; }
        public bool ActiveLog { get; set; }
        public EFProfilerUIOptions EFProfilerUIOptions { get; set; }
    }
}
