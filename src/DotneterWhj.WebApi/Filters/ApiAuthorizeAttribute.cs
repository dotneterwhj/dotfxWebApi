using DotneterWhj.Extensions;
using DotneterWhj.ToolKits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace DotneterWhj.WebApi
{
    /// <summary>
    /// 授权特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 处理未授权的请求
        /// </summary>
        /// <param name="actionContext"></param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            var messageModel = new MessageModel<string>
            {
                Data = null,
            };
            messageModel.Status = HttpStatusCode.Forbidden;
            messageModel.Msg = "您无权访问此接口";

            var reponse = messageModel.GetHttpResponseMessage();

            actionContext.Response = reponse;
        }
    }
}