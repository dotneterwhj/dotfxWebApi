using DotneterWhj.ToolKits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DotneterWhj.Extensions
{
    public static class MessageModelExtensions
    {
        public static HttpResponseMessage GetHttpResponseMessage<T>(this MessageModel<T> messageModel)
        {
            var reponse = new HttpResponseMessage()
            {
                Content = new ObjectContent(messageModel.GetType(), messageModel, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                StatusCode = messageModel.Status
            };

            return reponse;
        }
    }
}