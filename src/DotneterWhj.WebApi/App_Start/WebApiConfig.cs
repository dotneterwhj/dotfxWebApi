using DotneterWhj.Extensions;
using DotneterWhj.ToolKits;
using Microsoft.Web.Http;
using Microsoft.Web.Http.Versioning;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Tracing;
using Microsoft.Extensions.Options;

namespace DotneterWhj.WebApi
{
    /// <summary>
    /// webapi 配置
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            #region 跨域配置

            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            #endregion


            #region 版本控制

            config.AddApiVersioning(o =>
            {
                // 没有标记版本的action默认为5.0版本
                o.AssumeDefaultVersionWhenUnspecified = true;

                // 是否在请求头中返回受支持的版本信息
                o.ReportApiVersions = true;

                // 通过Header进行传值,//content-type来判断api的版本
                o.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"), new MediaTypeApiVersionReader());

                // 默认版本号
                o.DefaultApiVersion = new ApiVersion(5, 0);
            });

            var apiExplorer = config.AddVersionedApiExplorer(
                   options =>
                   {
                       options.GroupNameFormat = "'v'VVV";

                       // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                       // can also be used to control the format of the API version in route templates
                       options.SubstituteApiVersionInUrl = false;

                       options.AddApiVersionParametersWhenVersionNeutral = false;
                   });

            #endregion


            //config.Services.Replace(typeof(IHttpControllerSelector), new VersionControllerSelector(config));

            #region 异常处理程序

            // 以下情形中的异常，过滤器是无法捕获到的：使用IExceptionHandler可以捕获
            // Controller构造函数中抛出的异常
            // 消息处理器中抛出的异常
            // 路由过程中出现的异常
            // 其它过滤器中抛出的异常
            // 序列化返回内容时抛出的异常
            config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());

            #endregion


            #region 日志trace替换

            config.Services.Replace(typeof(ITraceWriter), new NLogTraceWriter());

            //SystemDiagnosticsTraceWriter traceWriter = config.EnableSystemDiagnosticsTracing();
            //traceWriter.IsVerbose = true;
            //traceWriter.MinimumLevel = TraceLevel.Debug;
            //config.Services.Replace(typeof(ITraceWriter), new WebApiTraceWriter());

            #endregion


            #region Filter配置

            config.Filters.Add(new JwtAuthentication());

            #endregion


            #region Web API 路由配置

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            #endregion


            #region Swagger 配置

            SwaggerConfig.Register(config, apiExplorer);

            #endregion

        }
    }
}