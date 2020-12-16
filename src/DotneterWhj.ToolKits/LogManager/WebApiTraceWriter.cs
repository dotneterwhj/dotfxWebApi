using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Tracing;

namespace DotneterWhj.ToolKits
{
    public class WebApiTraceWriter : ITraceWriter
    {
        public void Trace(HttpRequestMessage request, string category, TraceLevel level,
            Action<TraceRecord> traceAction)
        {
            TraceRecord rec = new TraceRecord(request, category, level);
            traceAction?.Invoke(rec);
            WriteTrace(rec);
        }

        protected void WriteTrace(TraceRecord rec)
        {
            var message = string.Format("{0};{1};{2}",
                rec.Operator, rec.Operation, rec.Message);
            System.Diagnostics.Trace.WriteLine(message, rec.Category);
        }
    }
}
