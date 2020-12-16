using Autofac.Integration.WebApi;
using DotneterWhj.DataTransferObject;
using DotneterWhj.Extensions;
using DotneterWhj.ToolKits;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;

namespace DotneterWhj.WebApi
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    public class WebApiExceptionFilter : IAutofacExceptionFilter // : ExceptionFilterAttribute
    {
        private readonly string Enviroment = ConfigurationManager.AppSettings[GlobalConstract.ASPNET_ENVIRONMENT];

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var traceWriter = GlobalConfiguration.Configuration.Services.GetTraceWriter();

            var wholeLink = (GlobalWholeLink)actionExecutedContext.Request.GetDependencyScope().GetService(typeof(GlobalWholeLink));

            // 1.异常日志记录
            traceWriter.Error(actionExecutedContext.Request,
                            "The whole link id : " + wholeLink.WholeLinkId.ToString() +
                            Environment.NewLine +
                            "Controller : " + actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName +
                            Environment.NewLine +
                            "Action : " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName +
                            Environment.NewLine +
                            "ActionArguments : " + JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionArguments),
                            actionExecutedContext.Exception);

            var isDevelopment = string.Equals(Enviroment, "Development", StringComparison.InvariantCultureIgnoreCase);

            // 2.返回调用方具体的异常信息
            if (actionExecutedContext.Exception is NotImplementedException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);

                var messageModel = new MessageModel<Exception>
                {
                    Data = isDevelopment ? actionExecutedContext.Exception : null,
                    Msg = actionExecutedContext.Exception.Message,
                    Status = HttpStatusCode.NotImplemented,
                };

                actionExecutedContext.Response = messageModel.GetHttpResponseMessage();

                //throw new HttpResponseException(reponse);
            }
            else if (actionExecutedContext.Exception is TimeoutException)
            {
                var messageModel = new MessageModel<Exception>
                {
                    Data = isDevelopment ? actionExecutedContext.Exception : null,
                    Msg = actionExecutedContext.Exception.Message,
                    Status = HttpStatusCode.RequestTimeout,
                };

                actionExecutedContext.Response = messageModel.GetHttpResponseMessage();

                //throw new HttpResponseException(reponse);
            }
            else if (actionExecutedContext.Exception is HttpResponseException)
            {
                var httpReponse = ((HttpResponseException)actionExecutedContext.Exception).Response;

                var statusCode = httpReponse.StatusCode;

                var messageModel = new MessageModel<Exception>
                {
                    Data = isDevelopment ? actionExecutedContext.Exception : null,
                    Msg = httpReponse.Content.ReadAsStringAsync().Result,
                    Status = statusCode,
                };

                actionExecutedContext.Response = messageModel.GetHttpResponseMessage();

                //throw new HttpResponseException(reponse);
            }
            else if (actionExecutedContext.Exception is ParamterNotExistException)
            {
                var messageModel = new MessageModel<Exception>
                {
                    Data = isDevelopment ? actionExecutedContext.Exception : null,
                    Status = HttpStatusCode.BadRequest,
                    Msg = actionExecutedContext.Exception.Message
                };

                actionExecutedContext.Response = messageModel.GetHttpResponseMessage();

                //throw new HttpResponseException(reponse);
            }
            // .....这里可以根据项目需要返回到客户端特定的状态码。如果找不到相应的异常，统一返回服务端错误500
            else
            {
                var messageModel = new MessageModel<Exception>
                {
                    Data = isDevelopment ? actionExecutedContext.Exception : null,
                    Msg = actionExecutedContext.Exception.Message,
                    Status = HttpStatusCode.InternalServerError,
                };

                actionExecutedContext.Response = messageModel.GetHttpResponseMessage();

                //throw new HttpResponseException(reponse);
            }
            return Task.FromResult(0);
        }

        #region webapi ExceptionFilterAttribute 实现

        ///// <summary>
        ///// 异常处理
        ///// </summary>
        ///// <param name="actionExecutedContext"></param>
        //public override void OnException(HttpActionExecutedContext actionExecutedContext)
        //{
        //    // 1.异常日志记录
        //    _traceWriter.Error(actionExecutedContext.Request,
        //                    "Controller : " + actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine +
        //                    "Action : " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName,
        //                    actionExecutedContext.Exception);

        //    // 2.返回调用方具体的异常信息
        //    if (actionExecutedContext.Exception is NotImplementedException)
        //    {
        //        actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
        //    }
        //    else if (actionExecutedContext.Exception is TimeoutException)
        //    {
        //        actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
        //    }
        //    // .....这里可以根据项目需要返回到客户端特定的状态码。如果找不到相应的异常，统一返回服务端错误500
        //    else
        //    {
        //        actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        //        var messageModel = new MessageModel<string>
        //        {
        //            Data = null,
        //            Msg = actionExecutedContext.Exception.Message,
        //            Status = HttpStatusCode.InternalServerError,
        //        };

        //        var reponse = new HttpResponseMessage()
        //        {
        //            Content = new ObjectContent(messageModel.GetType(), messageModel, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
        //            StatusCode = HttpStatusCode.InternalServerError
        //        };

        //        throw new HttpResponseException(reponse);
        //    }

        //    base.OnException(actionExecutedContext);
        //}

        ///// <summary>
        ///// 异常处理
        ///// </summary>
        ///// <param name="actionExecutedContext"></param>
        ///// <param name="cancellationToken"></param>
        ///// <returns></returns>
        //public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        //{
        //    OnException(actionExecutedContext);

        //    return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
        //}

        #endregion webapi ExceptionFilterAttribute 实现
    }
}