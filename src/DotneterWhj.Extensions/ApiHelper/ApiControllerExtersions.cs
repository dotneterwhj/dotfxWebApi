using DotneterWhj.ToolKits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotneterWhj.Extensions
{
    public static class ApiControllerExtersions
    {
        public static IHttpActionResult ErrorHttpActionResult(this ApiController apiController,
            HttpStatusCode httpStatusCode,
            string msg,
            Exception exception = null)
        {
            var messageModel = new MessageModel<Exception>();
            messageModel.Data = exception;
            messageModel.Msg = msg;
            messageModel.Status = httpStatusCode;

            var errorReponse = messageModel.GetHttpResponseMessage();

            throw new HttpResponseException(errorReponse);
        }
    }
}
