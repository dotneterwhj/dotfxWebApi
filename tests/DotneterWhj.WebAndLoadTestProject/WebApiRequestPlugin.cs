using lancoo.cp.basic.sysbaseclass;
using Microsoft.VisualStudio.TestTools.WebTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.WebAndLoadTestProject
{
    [Description("请求")]
    public class WebApiRequestPlugin:WebTestRequestPlugin
    {
        public override void PreRequest(object sender, PreRequestEventArgs e)
        {
            // e.Request.Url = e.Request.Url + "/growing/test/123";

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数

            var nonce = Guid.NewGuid().ToString();
            var sign = CP_MD5Helper.GetMd5Hash("e23815365a761e4b397ce30822a2d5bb" + timeStamp.ToString() + nonce);

            e.Request.Headers.Add("sign", sign);
            e.Request.Headers.Add("timestamp", timeStamp.ToString());
            e.Request.Headers.Add("nonce", nonce);
            e.Request.Headers.Add("appid", "930");
            e.Request.Headers.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiVGVhY2hlciIsInVuaXF1ZV9uYW1lIjoidzExMSIsIm5iZiI6MTYwNjcwMDc5NSwiZXhwIjoxNjEyNzAwNzk1LCJpYXQiOjE2MDY3MDA3OTV9.mbp9Ub7WZomFTGR2_7CIZua-7M7ponD5NiO-RiyIxRk");

        }
    }
}
