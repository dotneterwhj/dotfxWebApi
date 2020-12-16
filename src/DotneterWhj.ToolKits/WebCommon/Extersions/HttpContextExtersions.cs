using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Web;

namespace DotneterWhj.ToolKits
{
    /// <summary>
    /// 解决Asp.net Mvc中使用异步的时候HttpContext.Current为null的方法
    /// <remarks>
    /// http://www.cnblogs.com/pokemon/p/5116446.html
    /// </remarks>
    /// </summary>
    public static class HttpContextExtersions
    {
        /// <summary>
        /// 在同步上下文中查找当前会话<see cref="System.Web.HttpContext" />对象
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static HttpContext FindHttpContext(this SynchronizationContext context)
        {
            if (context == null)
            {
                return null;
            }
            var factory = GetFindApplicationDelegate(context);
            return factory?.Invoke(context).Context;
        }

        private static Func<SynchronizationContext, HttpApplication> GetFindApplicationDelegate(SynchronizationContext context)
        {
            Delegate factory = null;
            Type type = context.GetType();
            if (!type.FullName.Equals("System.Web.LegacyAspNetSynchronizationContext"))
            {
                return null;
            }
            //找到字段
            ParameterExpression sourceExpression = Expression.Parameter(typeof(SynchronizationContext), "context");
            //目前支持 System.Web.LegacyAspNetSynchronizationContext 内部类
            //查找        private HttpApplication _application 字段
            Expression sourceInstance = Expression.Convert(sourceExpression, type);
            FieldInfo applicationFieldInfo = type.GetField("_application", BindingFlags.NonPublic | BindingFlags.Instance);
            Expression fieldExpression = Expression.Field(sourceInstance, applicationFieldInfo);
            factory = Expression.Lambda<Func<SynchronizationContext, HttpApplication>>(fieldExpression, sourceExpression).Compile();

            //返回委托
            return ((Func<SynchronizationContext, HttpApplication>)factory);
        }

        /// <summary>
        /// 确定异步状态的上下文可用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static HttpContext Check(this HttpContext context)
        {
            return context ?? (context = SynchronizationContext.Current.FindHttpContext());
        }
    }
}