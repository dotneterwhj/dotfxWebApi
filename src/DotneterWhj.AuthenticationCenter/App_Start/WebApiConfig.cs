using System.Web.Http;

namespace DotneterWhj.AuthenticationCenter
{
    internal class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}"
            );
        }
    }
}