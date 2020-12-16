using Autofac;
using Autofac.Integration.WebApi;
using DotneterWhj.IServices;
using DotneterWhj.WebApi.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace DotneterWhj.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(JsonSerializerConfig.Register);
            GlobalConfiguration.Configure(AutofacConfig.Register);
            GlobalConfiguration.Configure(AutomapperConfig.Register);
        }
    }
}
