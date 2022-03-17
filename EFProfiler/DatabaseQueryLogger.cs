using EFProfiler.Model;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static  EFProfilerSetting _efProfilerSetting { get; set; }
        public DatabaseQueryLogger(IConfiguration configuration)
        {
            _efProfilerSetting = configuration.GetSection("EFProfilerSetting").Get<EFProfilerSetting>();
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            if (_efProfilerSetting.ActiveLog && eventData.Duration.Milliseconds > _efProfilerSetting.MaxMillisecond)
            {
                LogLongQuery(command, eventData);
            }
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            if (_efProfilerSetting.ActiveLog && eventData.Duration.Milliseconds > _efProfilerSetting.MaxMillisecond)
            {
                LogLongQuery(command, eventData);
            }
            return base.ReaderExecuted(command, eventData, result);
        }

        private void LogLongQuery(DbCommand command, CommandExecutedEventData evenData)
        {
            var pathFile = _efProfilerSetting.Path + DateTime.UtcNow.Ticks + ".json";
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
