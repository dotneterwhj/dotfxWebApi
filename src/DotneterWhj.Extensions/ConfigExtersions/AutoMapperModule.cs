using Autofac;
using AutoMapper;
using DotneterWhj.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Extensions.ConfigExtersions
{

    public class AutoMapperRegisterModule : Autofac.Module
    {
        private readonly IEnumerable<Assembly> assembliesToScan;

        public AutoMapperRegisterModule(IEnumerable<Assembly> assembliesToScan)
        {
            var assems = new Assembly[]
            {
                Assembly.Load(GlobalConstract.WebApiDll),
                Assembly.Load(GlobalConstract.ExtensionsDll),
            };

            this.assembliesToScan = assems;
        }

        public AutoMapperRegisterModule(params Assembly[] assembliesToScan) : this((IEnumerable<Assembly>)assembliesToScan) { }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembliesToScan = this.assembliesToScan as Assembly[] ?? this.assembliesToScan.ToArray();
            var allTypes = assembliesToScan
                          .Where(a => !a.IsDynamic && a.GetName().Name != nameof(AutoMapper))
                          .Distinct() // avoid AutoMapper.DuplicateTypeMapConfigurationException
                          .SelectMany(a => a.DefinedTypes)
                          .Where(w => w.IsAssignableFrom(typeof(Profile)))//默认继承IProfile,排除不需要configuration的实例
                          .ToArray();

            var openTypes = new[] {
                            typeof(IValueResolver<,,>),
                            typeof(IMemberValueResolver<,,,>),
                            typeof(ITypeConverter<,>),
                            //typeof(IValueConverter<,>),
                            typeof(IMappingAction<,>)
            };

            foreach (var type in openTypes.SelectMany(openType =>
                 allTypes.Where(t => t.IsClass && !t.IsAbstract && ImplementsGenericInterface(t.AsType(), openType))))
            {
                builder.RegisterType(type.AsType()).InstancePerDependency();
            }


            //configuration配置
            builder.Register<IConfigurationProvider>(ctx =>
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(assembliesToScan);
                cfg.AllowNullCollections = true;//允许空集合
                //cfg.ForAllMaps((a, b) => b.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null)));
            })
            );

            builder.Register<IMapper>(ctx => new Mapper(ctx.Resolve<IConfigurationProvider>(), ctx.Resolve)).InstancePerDependency();
        }

        private static bool ImplementsGenericInterface(Type type, Type interfaceType)
                  => IsGenericType(type, interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => IsGenericType(@interface, interfaceType));

        private static bool IsGenericType(Type type, Type genericType)
                   => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }

}
