using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Tracing;

namespace DotneterWhj.ToolKits
{
    /// <summary>
    /// 使用Nlog来记录系统的日志
    /// </summary>
    public class NLogTraceWriter : ITraceWriter
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<Dictionary<TraceLevel, Action<string>>> _loggingMap = new Lazy<Dictionary<TraceLevel, Action<string>>>
            (() => new Dictionary<TraceLevel, Action<string>>
            {
                { TraceLevel.Info, _logger.Info },
                { TraceLevel.Debug, _logger.Debug },
                { TraceLevel.Error, _logger.Error },
                { TraceLevel.Fatal, _logger.Fatal },
                { TraceLevel.Warn, _logger.Warn }
            });

        private Dictionary<TraceLevel, Action<string>> Logger
        {
            get { return _loggingMap.Value; }
        }

        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if (level != TraceLevel.Off) //如果没用禁用日志跟踪
            {
                //if (traceAction != null && traceAction.Target != null)
                //{
                //    category = category + Environment.NewLine + "Action Parameters : " + JsonConvert.SerializeObject(traceAction.Target);
                //}

                var record = new TraceRecord(request, category, level);

                traceAction?.Invoke(record);

                Log(record);
            }
        }

        private void Log(TraceRecord record)
        {
            var message = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(record.Message))
                message.Append("").Append(record.Message + Environment.NewLine);

            if (record.Request != null)
            {
                if (record.Request.Method != null)
                    message.Append("Method: " + record.Request.Method + Environment.NewLine);

                if (record.Request.RequestUri != null)
                    message.Append("").Append("URL: " + record.Request.RequestUri + Environment.NewLine);

                if (record.Request.Headers != null && record.Request.Headers.Contains("Token") && record.Request.Headers.GetValues("Token") != null && record.Request.Headers.GetValues("Token").FirstOrDefault() != null)
                    message.Append("").Append("Token: " + record.Request.Headers.GetValues("Token").FirstOrDefault() + Environment.NewLine);
            }

            if (!string.IsNullOrWhiteSpace(record.Category))
                message.Append("").Append(record.Category);

            if (!string.IsNullOrWhiteSpace(record.Operator))
                message.Append(" ").Append(record.Operator).Append(" ").Append(record.Operation);

            //***************************异常日志***********************************//
            if (record.Exception != null && !string.IsNullOrWhiteSpace(record.Exception.GetBaseException().Message))
            {
                var exceptionType = record.Exception.GetType();
                message.Append(Environment.NewLine);
                message.Append("").Append("Error : " + record.Exception.GetBaseException().Message + Environment.NewLine);
            }

            // 日志写入本地文件
            Logger[record.Level](Convert.ToString(message) + Environment.NewLine);
        }
    }
}
