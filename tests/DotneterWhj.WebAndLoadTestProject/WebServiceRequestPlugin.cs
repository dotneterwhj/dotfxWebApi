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
    public class WebServiceRequestPlugin : WebTestRequestPlugin
    {
        public class VerifyClass
        {
            /// <summary>
            /// 单例
            /// </summary>
            private static object obj = new object();

            /// <summary>
            /// webservice访问用户名  
            /// </summary>
            private string _uname = string.Empty;

            /// <summary>
            /// webservice访问密码  
            /// </summary>
            private string _password = string.Empty;

            public string Uname
            {
                get { return _uname; }
                set { _uname = value; }
            }

            public string Password
            {
                get { return _password; }
                set { _password = value; }
            }

            private static VerifyClass verifyClass;
            private VerifyClass()
            {
                //  
                //TODO: 在此处添加构造函数逻辑  
                //  
            }
            public VerifyClass(string uname, string upass)
            {
                init(uname, upass);
            }
            private void init(string uname, string upass)
            {
                this._password = upass;
                this._uname = uname;
            }
        }



        public override void PreRequest(object sender, PreRequestEventArgs e)
        {
            // e.Request.Url = e.Request.Url + "/growing/test/123";

            //ht.Add("JiaoCaiID", JiaoCaiID);
            //ht.Add("StagingStatus", StagingStatus);
            //ht.Add("editstatus", editstatus);
            //ht.Add("verifyClass", VerifyClass.GetInstance());
            var verifyClass = new VerifyClass("LancooEditBook", "LancooEditBook");
            e.Request.QueryStringParameters.Add("JiaoCaiID","");
            e.Request.QueryStringParameters.Add("StagingStatus", "");
            e.Request.QueryStringParameters.Add("editstatus", "");
            e.Request.QueryStringParameters.Add("verifyClass", verifyClass.ToString());
        }
    }
}
