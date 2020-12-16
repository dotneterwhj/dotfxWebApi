using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.ToolKits
{
    public static class ObjectExtersions
    {
        /// <summary>
        /// 对象塑形
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="fields">多个以英文逗号,相隔</param>
        /// <returns></returns>
        public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObj = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos =
                    typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public |
                                                  BindingFlags.Instance);
                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)expandoObj).Add(propertyInfo.Name, propertyValue);
                }
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');
                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    var propertyInfo = typeof(TSource).GetProperty(propertyName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new ParamterNotExistException($"{typeof(TSource).Name}不存在{propertyName}该属性");
                    }

                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)expandoObj).Add(propertyInfo.Name, propertyValue);
                }
            }

            return expandoObj;
        }
    }
}
