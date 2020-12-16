using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DotneterWhj.Extensions
{
    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(HttpRequestMessage request, object value)
        {
            Request = request;
            Value = value;
        }
        public HttpRequestMessage Request { get; set; }

        public object Value { get; set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            response.Content = new ObjectContent(Value.GetType(), Value, GlobalConfiguration.Configuration.Formatters.JsonFormatter);

            response.RequestMessage = Request;

            return Task.FromResult(response);
        }
    }
}