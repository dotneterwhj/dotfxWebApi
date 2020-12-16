using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace DotneterWhj.DataTransferObject.ValidationAttributes
{
    /// <summary>
    /// 用于不同类中的属性值验证相等
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SameValueAttribute : ValidationAttribute
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="fieldNames">格式: 当前类的属性名称:属性类中的属性字段名称</param>
        public SameValueAttribute(string fieldNames)
        {
            if (string.IsNullOrEmpty(fieldNames)) throw new ArgumentException($"参数fieldNames不能为空");

            if (!fieldNames.Contains(":")) throw new ArgumentException($"参数fieldNames必须包含:,:之前为当前属性名称，:之后为指定的属性类中的属性名称");

            OtherPropertyNames = fieldNames;
        }

        /// <summary>
        /// 指定/获取自身要与其他相同的属性名称
        /// </summary>
        public string SelfPropertyName { get; set; }

        /// <summary>
        /// 获取其他与SelfPropertyName相同的属性名称
        /// </summary>
        public string OtherPropertyNames { get; }

        public override bool IsValid(object value)
        {
            return base.IsValid(value);
        }

        //[Compare]
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // 获取当前属性
            PropertyInfo selfPropertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            // 获取标记SameValueAttribute需要验证的值
            var selfValue = selfPropertyInfo.PropertyType.GetProperty(SelfPropertyName).GetValue(value, null);

            // 获取标记SameValueAttribute需要验证的其他属性
            var otherPropertyNamesArray = OtherPropertyNames.Split(',');

            foreach (var otherProperty in otherPropertyNamesArray)
            {
                PropertyInfo propertyInfo = validationContext.ObjectType.GetProperty(otherProperty.Split(':')[0]);

                if (propertyInfo == null)
                {
                    return new ValidationResult(string.Format(CultureInfo.CurrentCulture, "未知属性", otherProperty.Split(':')[0]));
                }

                var otherPropertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

                var enumerable = otherPropertyValue.GetType().GetInterface("IEnumerable", false);

                // 是集合的情况
                if (enumerable != null)
                {
                    foreach (var item in (IEnumerable<object>)otherPropertyValue)
                    {
                        // 获取标记SameValueAttribute需要验证的其他 值
                        var otherPropertyValue1 = item.GetType().GetProperty(otherProperty.Split(':')[1]).GetValue(item, null);

                        if (!object.Equals(selfValue, otherPropertyValue1))
                        {
                            return new ValidationResult(FormatErrorMessage(""));
                        }
                    }
                }
                // 不是集合
                else
                {
                    var otherPropertyValue1 = otherPropertyValue.GetType().GetProperty(otherProperty.Split(':')[1]).GetValue(otherPropertyValue, null);

                    if (!object.Equals(selfValue, otherPropertyValue1))
                    {
                        return new ValidationResult(FormatErrorMessage(""));
                    }
                }
            }

            return null;
        }
    }
}