using DotneterWhj.DataTransferObject;
using DotneterWhj.ToolKits;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Tracing;

namespace DotneterWhj.WebApi
{
    internal class CustomExceptionHandler : ExceptionHandler
    {
        private readonly string Enviroment = ConfigurationManager.AppSettings[GlobalConstract.ASPNET_ENVIRONMENT];

        public override void Handle(ExceptionHandlerContext context)
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogTraceWriter());

            var traceWriter = GlobalConfiguration.Configuration.Services.GetTraceWriter();

            var wholeLink = (GlobalWholeLink)context.Request.GetDependencyScope().GetService(typeof(GlobalWholeLink));

            traceWriter.Error(context.Request,
                "The whole link id : " + wholeLink.WholeLinkId.ToString() +
                Environment.NewLine +
                "RequestUri : " + context.Request.RequestUri +
                Environment.NewLine +
                "Content : " + context.Request.Content,
                context.Exception);

            var isDevelopment = string.Equals(Enviroment, "Development", StringComparison.InvariantCultureIgnoreCase);

            var messageModel = new MessageModel<Exception>
            {
                Data = isDevelopment ? context.Exception : null,
                Msg = context.Exception.Message,
                Status = HttpStatusCode.InternalServerError,
            };

            context.Result = new TextPlainErrorResult
            {
                Request = context.ExceptionContext.Request,
                Value = messageModel
            };
        }

        private class TextPlainErrorResult : IHttpActionResult
        {
            public HttpRequestMessage Request { get; set; }

            public object Value { get; set; }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage response =
                                 new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new ObjectContent(Value.GetType(), Value, GlobalConfiguration.Configuration.Formatters.JsonFormatter);
                response.RequestMessage = Request;
                return Task.FromResult(response);
            }
        }
    }
}