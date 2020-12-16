using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace DotneterWhj.WebApi
{
    /// <summary>
    /// 向Swagger添加枚举值说明
    /// </summary>
    public class EnumDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiExplorer"></param>
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            // 向结果模型添加枚举描述
            foreach (var schemaDictionaryItem in swaggerDoc.definitions)
            {
                var schema = schemaDictionaryItem.Value;
                foreach (var propertyDictionaryItem in schema.properties)
                {
                    var property = propertyDictionaryItem.Value;
                    var propertyEnums = property.@enum;
                    if (propertyEnums != null && propertyEnums.Count > 0)
                    {
                        property.description += DescribeEnum(propertyEnums);
                    }
                }
            }
        }

        /// <summary>
        /// 描述枚举
        /// </summary>
        /// <param name="enums"></param>
        /// <returns></returns>
        private static string DescribeEnum(IEnumerable<object> enums)
        {
            var enumDescriptions = new List<string>();
            Type type = null;
            foreach (var enumOption in enums)
            {
                if (type == null) type = enumOption.GetType();
                enumDescriptions.Add($"{Convert.ChangeType(enumOption, type.GetEnumUnderlyingType())} = {Enum.GetName(type, enumOption)}，{GetDescription(type, enumOption)}");
            }

            return $"{Environment.NewLine}{string.Join(Environment.NewLine, enumDescriptions)}";
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="t"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetDescription(Type t, object value)
        {
            foreach (MemberInfo mInfo in t.GetMembers())
            {
                if (mInfo.Name == t.GetEnumName(value))
                {
                    foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                    {
                        if (attr.GetType() == typeof(DescriptionAttribute))
                        {
                            return ((DescriptionAttribute)attr).Description;
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
