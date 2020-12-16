using Autofac;
using Autofac.Integration.WebApi;
using DotneterWhj.Extensions;
using DotneterWhj.Extensions.ConfigExtersions;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Web.Http;

namespace DotneterWhj.WebApi.App_Start
{
    /// <summary>
    /// autofac容器配置
    /// </summary>
    public class AutofacConfig
    {
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            var assem = Assembly.GetExecutingAssembly();

            //// Get your HttpConfiguration.
            //var config = GlobalConfiguration.Configuration;

            // 注册相应模块
            builder.RegisterModule(new AutofacRegisterModule());

            //automapper配置
            builder.RegisterModule(new AutoMapperRegisterModule());

            // Register your Web API controllers.
            builder.RegisterApiControllers(assem);

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            #region 注册配置文件信息

            builder.RegisterConfiguration(config);

            #endregion

            builder.RegisterGeneric(typeof(OptionsManager<>)).As(typeof(IOptions<>)).SingleInstance();
            builder.RegisterGeneric(typeof(OptionsMonitor<>)).As(typeof(IOptionsMonitor<>));
            builder.RegisterGeneric(typeof(OptionsSnapshot<>)).As(typeof(IOptionsSnapshot<>));

            // 异常处理
            builder.Register(c => new WebApiExceptionFilter())
                   .AsWebApiExceptionFilterForAllControllers()
                   .InstancePerRequest();

            // 同一类型的filter与注册的顺序相关，先注册的先执行
            // header参数验证
            //builder.Register(c => new ParametersSecurityFilter())
            //        .AsWebApiActionFilterForAllControllers()
            //        .InstancePerRequest();

            // 模型验证
            builder.Register(c => new ModelStateValidFilter())
                    .AsWebApiActionFilterForAllControllers()
                    .InstancePerRequest();

            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

        }
    }
}