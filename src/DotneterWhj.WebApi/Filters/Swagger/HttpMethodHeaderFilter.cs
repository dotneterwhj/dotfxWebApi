using Autofac.Integration.WebApi;
using Microsoft.Web.Http;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Filters;

namespace DotneterWhj.WebApi
{
    /// <summary>
    /// 请求header设置
    /// </summary>
    public class HttpMethodHeaderFilter : IOperationFilter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            var queryApiVersion = operation.parameters.FirstOrDefault(p => p.name == "api-version" && p.@in == "query");
            if (queryApiVersion != null)
            {
                operation.parameters.Remove(queryApiVersion);
            }

            var filters = apiDescription.ActionDescriptor.GetFilterPipeline(); // 获取管道中的所有过滤器

            var isNeedAuthenticate = filters.Select(filterInfo => filterInfo.Instance)
                                             .Any(filter => filter is IAuthenticationFilter); // 全局过滤器中是否具有身份认证过滤器

            var isNeedParametersSecurityValidate = filters.Select(filterInfo => filterInfo.Instance)
                                             .Where(filter => filter is IAutofacContinuationActionFilter)
                                             .Where(filter => filter is ParametersSecurityFilter); // 全局过滤器中是否具有header参数验证过滤器

            if (isNeedParametersSecurityValidate.Count() > 0)
            {
                var paramterSec = new List<Parameter>
                {
                    new Parameter{ name = "sign", @in = "header", description = "签名(appsecret timestamp nonce 三个字段顺序直接相拼接计算的md5值)", required = true, type = "string" },
                    new Parameter{ name = "timestamp", @in = "header", description = "时间戳", required = true, type = "string" },
                    new Parameter{ name = "nonce", @in = "header", description = "随机数,不可重复", required = true, type = "string" },
                    new Parameter{ name = "appkey", @in = "header", description = "appkey,需调用方向教材编辑申请获得", required = true, type = "string" },
                };
                foreach (var item in paramterSec)
                {
                    operation.parameters.Add(item);
                }
            }

            var authorization = new Parameter { name = "Authorization", @in = "header", description = "认证", required = true, type = "string" };

            if (isNeedAuthenticate)
            {
                operation.parameters.Add(authorization);
            }

            // var actionAuthorizeFilter = apiDescription.ActionDescriptor.GetCustomAttributes<AuthorizeAttribute>().Any();
            // var controllerFilter = apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AuthorizeAttribute>(true).Any();

            // 如果包含名字叫JwtAuthenticationAttribute过滤器则给该方法的swagger调用出添加两个头部信息输入框(看个人需要)
            // if (actionAuthorizeFilter || controllerFilter)
            // {
            //     operation.parameters.Add(authorization);
            // }

            var allowAnonymous = apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

            if (allowAnonymous)
            {
                if (operation.parameters.Contains(authorization))
                    operation.parameters.Remove(authorization);
            }

            var versionActionFilter = apiDescription.ActionDescriptor.GetCustomAttributes<ApiVersionAttribute>().Any();
            var versionControllerFilter = apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<ApiVersionAttribute>(true).Any();
            if (versionControllerFilter || versionActionFilter)
            {
                operation.parameters.Add(new Parameter { name = "api-version", @in = "header", description = "版本号", required = false, type = "string" });
            }
        }
    }
}