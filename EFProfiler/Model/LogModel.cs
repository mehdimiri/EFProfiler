using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFProfiler.Model
{
    public class LogModel
    {
        public DateTime LogDate { get; set; } = DateTime.UtcNow;
        public string Query { get; set; }
        public double TotalMilliseconds { get; set; }
        public double TotalSeconds { get; set; }
        public string Database { get; set; }
    }
}
