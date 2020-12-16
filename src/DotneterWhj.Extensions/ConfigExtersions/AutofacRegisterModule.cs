using Autofac;
using DotneterWhj.Caching;
using DotneterWhj.DataTransferObject;
using DotneterWhj.EntityFramework.EntityFramework;
using DotneterWhj.Repository;
using DotneterWhj.Repository.UnitOfWork;
using DotneterWhj.ToolKits;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using WebApi.OutputCache.Core.Cache;

namespace DotneterWhj.Extensions
{
    public class AutofacRegisterModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<TestDbContext>().As<DbContext>().InstancePerLifetimeScope();

            builder.RegisterType<EFUnitOfWork>().As<IUnitOfWork>();

            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>));
            // builder.RegisterType<CategoryRepository>().As<IRepository<category_child>>();

            // 获取 Repository.dll 程序集服务，并注册
            var assemblysRepository = Assembly.Load(GlobalConstract.RepositoryDll);
            builder.RegisterAssemblyTypes(assemblysRepository)
                   //.Where(c => c.Name.EndsWith("Repository"))
                   .Where(t => t.IsAssignableTo<IRepository>())
                   .AsImplementedInterfaces()
                   .InstancePerDependency();

            builder.RegisterAssemblyTypes(Assembly.Load(GlobalConstract.IServicesDll),
                                          Assembly.Load(GlobalConstract.ServicesDll))
                   .AsImplementedInterfaces();

            builder.RegisterType(typeof(GlobalWholeLink)).InstancePerLifetimeScope();

            builder.Register<ConnectionMultiplexer>(r =>
            {
                return ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings[GlobalConstract.RedisConnectionStr]);
            }).AsSelf().SingleInstance();

            builder.RegisterType<RedisCacheProvider>().As<IApiOutputCache>();

            #region automapper注入 已转移至 AutoMaopperModeul
            //builder.Register<IMapper>(r =>
            //{
            //    var mapperConfiguration = new MapperConfiguration(c =>
            //    {
            //        c.AddProfile(new OrganizationProfile());
            //    });
            //    mapperConfiguration.AssertConfigurationIsValid();

            //    return new Mapper(mapperConfiguration);
            //});
            #endregion

            builder.RegisterGeneric(typeof(NLogger<>)).As(typeof(ICustomLogger<>));

        }
    }
}