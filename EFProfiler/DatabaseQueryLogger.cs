using EFProfiler.Model;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EFProfiler
{
    public class DatabaseQueryLogger : DbCommandInterceptor
    {
        private int maxMillisecond { get; set; } = 0;
        private string path { get; set; }
        private bool activeLog { get; set; }
        public DatabaseQueryLogger(int maxMillisecond, string path, bool activeLog = true)
        {
            this.maxMillisecond = maxMillisecond;
            this.path = path;
            this.activeLog = activeLog;
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            if (activeLog && eventData.Duration.Milliseconds > maxMillisecond)
            {
                LogLongQuery(command, eventData);
            }
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            if (activeLog && eventData.Duration.Milliseconds > maxMillisecond)
            {
                LogLongQuery(command, eventData);
            }
            return base.ReaderExecuted(command, eventData, result);
        }

        private void LogLongQuery(DbCommand command, CommandExecutedEventData evenData)
        {
            var pathFile = path + DateTime.UtcNow.Ticks + ".json";
            using (FileStream fileStream = System.IO.File.Create(pathFile))
            {
                LogModel vm = new LogModel()
                {
                    Query = command.CommandText,
                    TotalSeconds = evenData.Duration.TotalSeconds,
                    TotalMilliseconds = evenData.Duration.TotalMilliseconds,
                    Database = command.Connection.Database,
                };
                string txt = JsonSerializer.Serialize(vm);
                byte[] content = new UTF8Encoding(true).GetBytes(txt);
                fileStream.Write(content, 0, content.Length);
            }
        }
    }
}
