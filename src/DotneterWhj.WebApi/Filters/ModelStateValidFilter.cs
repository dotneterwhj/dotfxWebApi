using Autofac.Integration.WebApi;
using DotneterWhj.Extensions;
using DotneterWhj.ToolKits;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using System.Web.Http.Tracing;

namespace DotneterWhj.WebApi
{
    /// <summary>
    /// 模型验证特性
    /// </summary>
    public class ModelStateValidFilter : IAutofacActionFilter // ActionFilterAttribute
    {
        #region webapi ActionfilterAttribute

        //    //
        //    // 摘要:
        //    //     Occurs before the action method is invoked.
        //    //
        //    // 参数:
        //    //   actionContext:
        //    //     The action context.
        //    /// <summary>
        //    /// Action执行前调用
        //    /// </summary>
        //    /// <param name="actionContext"></param>
        //    public override void OnActionExecuting(HttpActionContext actionContext)
        //    {
        //        GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogHelper());
        //        var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
        //        trace.Info(actionContext.Request, "Controller : " + actionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action : " + actionContext.ActionDescriptor.ActionName, "JSON", actionContext.ActionArguments);

        //        if (actionContext.ModelState.IsValid)
        //        {
        //        }
        //        else
        //        {
        //            var messageModel = new MessageModel<ModelStateDictionary>
        //            {
        //                Data = actionContext.ModelState,
        //            };
        //            messageModel.Status = HttpStatusCode.BadRequest;
        //            messageModel.Msg = "无效的请求";

        //            var reponse = new HttpResponseMessage()
        //            {
        //                Content = new ObjectContent(messageModel.GetType(), messageModel, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
        //                StatusCode = HttpStatusCode.BadRequest
        //            };

        //            actionContext.Response = reponse;
        //        }
        //    }

        //    //
        //    // 摘要:
        //    //     Occurs after the action method is invoked.
        //    //
        //    // 参数:
        //    //   actionExecutedContext:
        //    //     The action executed context.
        //    /// <summary>
        //    /// Action执行后
        //    /// </summary>
        //    /// <param name="actionExecutedContext"></param>
        //    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        //    {
        //        // 取得由 API 返回的资料
        //        //var result = actionExecutedContext.ActionContext.Response.Content.ReadAsHttpResponseMessageAsync();

        //        //if (result.GetType() == typeof(JsonResult<>))
        //        {
        //            ////请求是否成功
        //            //result.IsSuccess = actionExecutedContext.ActionContext.Response.IsSuccessStatusCode;
        //            ////结果转为自定义消息格式
        //            //HttpResponseMessage httpResponseMessage = JsonHelper.toJson(result);
        //            //// 重新封装回传格式
        //            //actionExecutedContext.Response = httpResponseMessage;
        //        }

        //    }

        //    /// <summary>
        //    /// 异步 Action执行前
        //    /// </summary>
        //    /// <param name="actionContext"></param>
        //    /// <param name="cancellationToken"></param>
        //    /// <returns></returns>
        //    public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        //    {
        //        OnActionExecuting(actionContext);
        //        return Task.FromResult(0);
        //    }

        //    /// <summary>
        //    /// 异步 Action执行后
        //    /// </summary>
        //    /// <param name="actionExecutedContext"></param>
        //    /// <param name="cancellationToken"></param>
        //    /// <returns></returns>
        //    public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        //    {
        //        OnActionExecuted(actionExecutedContext);
        //        return Task.FromResult(0);
        //    }

        #endregion webapi ActionfilterAttribute

        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            // 查找不进行模型验证的特性 直接返回
            var ignoreModelStateValidAttribute = actionContext.ActionDescriptor.GetCustomAttributes<IgnoreModelStateValidAttribute>();

            if (ignoreModelStateValidAttribute != null && ignoreModelStateValidAttribute.Count > 0)
            {
                return Task.FromResult(0);
            }

            var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();

            var wholeLink = (GlobalWholeLink)actionContext.Request.GetDependencyScope().GetService(typeof(GlobalWholeLink));

            if (actionContext.ModelState.IsValid)
            {
                trace.Info(actionContext.Request,
                    "The whole link id : " + wholeLink.WholeLinkId.ToString() +
                    Environment.NewLine +
                    "Controller : " + actionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName +
                    Environment.NewLine +
                    "Action : " + actionContext.ActionDescriptor.ActionName +
                    Environment.NewLine +
                    "ActionArguments : " + JsonConvert.SerializeObject(actionContext.ActionArguments),
                    "JSON",
                    actionContext.ActionArguments);

                if (actionContext.ActionArguments != null)
                {
                    foreach (var argument in actionContext.ActionArguments)
                    {
                        if (argument.Value == null)
                        {
                            var messageModel = new MessageModel<string>
                            {
                                Data = $"参数{argument.Key}是必须的",
                            };

                            messageModel.Status = HttpStatusCode.BadRequest;

                            messageModel.Msg = "无效的请求";

                            var reponse = messageModel.GetHttpResponseMessage();

                            actionContext.Response = reponse;

                            return Task.FromResult(0);
                        }
                    }
                }
            }
            else
            {
                trace.Warn(actionContext.Request,
                    "The whole link id :" + wholeLink.WholeLinkId.ToString() +
                    Environment.NewLine +
                    "Controller : " + actionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName +
                    Environment.NewLine +
                    "Action : " + actionContext.ActionDescriptor.ActionName +
                    Environment.NewLine +
                    "ActionArguments : " + JsonConvert.SerializeObject(actionContext.ActionArguments) +
                    Environment.NewLine +
                    "ModelState : " + JsonConvert.SerializeObject(actionContext.ModelState),
                    "JSON",
                    actionContext.ActionArguments);

                var messageModel = new MessageModel<ModelStateDictionary>
                {
                    Data = actionContext.ModelState,
                };
                messageModel.Status = HttpStatusCode.BadRequest;
                messageModel.Msg = "无效的请求";

                var reponse = messageModel.GetHttpResponseMessage();

                actionContext.Response = reponse;
            }

            return Task.FromResult(0);
        }
    }
}