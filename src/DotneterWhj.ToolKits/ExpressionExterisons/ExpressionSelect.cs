using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.ToolKits
{
    public class ExpressionSelect
    {
        public static Expression<Func<TSource, TResult>> CreateSelecter<TSource, TResult>(TypeMap typeMap, List<string> fields)
        {
            Expression<Func<TSource, TResult>> selector = null;

            //(rec)
            ParameterExpression param = Expression.Parameter(typeof(TSource), "x");

            //new ParadigmSearchListData 
            var newTResult = Expression.New(typeof(TResult));

            //Number
            List<MemberBinding> bindingList = new List<MemberBinding>();

            foreach (var field in fields)
            {
                var left = typeof(TResult).GetProperty(field, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

                // 找不到字段，则直接抛出异常
                if (left == null)
                {
                    throw new ParamterNotExistException($"{field}字段不存在");
                }

                var sourceName = left.Name;

                var propertyMaps = typeMap.GetPropertyMaps();

                // 找到TResult类型中的属性
                var propertyMap = propertyMaps
                    .Where(s => s.SourceMember.Name == left.Name)
                    .FirstOrDefault();

                // 解析TSource映射的属性名称
                if (propertyMap != null)
                {
                    sourceName = propertyMap.DestinationProperty.Name;
                }

                // Expression right = GetProperty<TSource>(null, field, param);
                var right = GetProperty<TSource>(null, sourceName, param);
                var express = Expression.Bind(left, right);
                bindingList.Add(express);
            }

            Expression body = Expression.MemberInit(newTResult, bindingList);

            selector = (Expression<Func<TSource, TResult>>)Expression.Lambda(body, param);

            return selector;
        }

        private static Expression GetProperty<T>(Expression source, string name, ParameterExpression Param)
        {
            name = name.Replace(")", "");
            string[] propertys = null;
            if (name.Contains("=>"))
            {
                propertys = name.Split('.').Skip(1).ToArray();
            }
            else
            {
                propertys = name.Split('.');
            }
            if (source == null)
            {
                source = Expression.Property(Param, typeof(T).GetProperty(propertys.First()));
            }
            else
            {
                source = Expression.Property(source, propertys.First());
            }
            foreach (var item in propertys.Skip(1))
            {
                source = GetProperty<T>(source, item, Param);
            }
            return source;
        }
    }
}
