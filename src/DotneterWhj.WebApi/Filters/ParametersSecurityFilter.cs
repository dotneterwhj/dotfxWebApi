using Autofac.Integration.WebApi;
using DotneterWhj.Extensions;
using DotneterWhj.IServices;
using DotneterWhj.ToolKits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;

namespace DotneterWhj.WebApi
{
    /// <summary>
    /// 参数安全验证特性
    /// </summary>
    public class ParametersSecurityFilter : IAutofacActionFilter
    {
        public Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            string sign = string.Empty, timestamp = string.Empty, nonce = string.Empty, appKey = string.Empty;

            if (actionContext.Request.Headers.TryGetValues("sign", out IEnumerable<string> signHeaders))
            {
                sign = signHeaders.First();
            }

            if (actionContext.Request.Headers.TryGetValues("timestamp", out IEnumerable<string> timestampHeaders))
            {
                timestamp = timestampHeaders.First();
            }

            if (actionContext.Request.Headers.TryGetValues("nonce", out IEnumerable<string> nonceHeaders))
            {
                nonce = nonceHeaders.First();
            }

            if (actionContext.Request.Headers.TryGetValues("appkey", out IEnumerable<string> appidHeaders))
            {
                appKey = appidHeaders.First();
            }

            var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();

            var wholeLink = (GlobalWholeLink)actionContext.Request.GetDependencyScope().GetService(typeof(GlobalWholeLink));

            trace.Debug(actionContext.Request,
                    "The whole link id : " + wholeLink.WholeLinkId.ToString() +
                    Environment.NewLine +
                    "sign : " + sign +
                    Environment.NewLine +
                    "timestamp : " + timestamp +
                    Environment.NewLine +
                    "nonce : " + nonce,
                    Environment.NewLine +
                    "appkey : " + appKey,
                    "JSON",
                    actionContext.ActionArguments);

            #region 验证参数的合法性

            var messageModel = new MessageModel<string>
            {
                Status = HttpStatusCode.BadRequest,
                Msg = "无效的请求"
            };

            if (string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(nonce) || string.IsNullOrEmpty(appKey))
            {
                messageModel.Data = "请求头headers中缺少相关的验证信息";

                var reponse = messageModel.GetHttpResponseMessage();

                actionContext.Response = reponse;

                return;
            }

            var appService = (IAppInfoService)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAppInfoService));

            var appInfo = (await appService.QueryAsync(q => q.AppId == appKey)).FirstOrDefault();

            if (appInfo == null || !appInfo.IsEnable)
            {
                messageModel.Data = "请求头headers中的appid不合法";

                var reponse = messageModel.GetHttpResponseMessage();

                actionContext.Response = reponse;

                return;
            }

            if (long.TryParse(timestamp, out long requestTime))
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区

                DateTime checkTime;
                if (timestamp.Length == 13) //时间戳是自 1970 年 1 月 1 日（00:00:00 GMT）以来的毫秒数
                {
                    checkTime = startTime.AddMilliseconds(requestTime);
                }
                else // Unix时间戳时间戳是自 1970 年 1 月 1 日（00:00:00 GMT）以来的秒数
                {
                    checkTime = startTime.AddSeconds(requestTime);
                }

                // 判断时间是不是过期了
                if (checkTime.AddSeconds(5 * 60) < DateTime.Now)
                {
                    messageModel.Data = "请求头headers中的timestamp过期";

                    var reponse = messageModel.GetHttpResponseMessage();

                    actionContext.Response = reponse;

                    return;
                }
            }
            else
            {
                messageModel.Data = "请求头headers中的timestamp不合法";

                var reponse = messageModel.GetHttpResponseMessage();

                actionContext.Response = reponse;

                return;
            }

            var computedSign = Md5Helper.GetMD5Hash(appInfo.AppSecret + timestamp + nonce);

            if (!sign.Equals(computedSign, StringComparison.CurrentCultureIgnoreCase))
            {
                messageModel.Data = "参数可能被篡改,无法处理该请求";

                var reponse = messageModel.GetHttpResponseMessage();

                actionContext.Response = reponse;

                return;
            }

            #endregion 验证参数的合法性
        }
    }
}