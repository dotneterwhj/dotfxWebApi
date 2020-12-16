using Autofac;
using DotneterWhj.DataTransferObject;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Web.Http;

namespace DotneterWhj.Extensions
{
    public static class AutofacContainerBuilderExtersions
    {
        public static void RegisterConfiguration(this ContainerBuilder builder, HttpConfiguration config)
        {
            builder.Register<IConfigureOptions<AppSettings>>(s => new ConfigureOptions<AppSettings>(options =>
            {
                options.Connection = ConfigurationManager.ConnectionStrings[GlobalConstract.Connection].ConnectionString;
                options.RedisConnectionStr = ConfigurationManager.AppSettings[GlobalConstract.RedisConnectionStr];
                options.ASPNET_ENVIRONMENT = ConfigurationManager.AppSettings[GlobalConstract.ASPNET_ENVIRONMENT];
            })).AsSelf().SingleInstance();
        }
    }
}