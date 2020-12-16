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
    public class ExpressionConverter<TFrom, TTo> : ExpressionVisitor
    where TFrom : class, new()
    where TTo : class, new()
    {
        //private readonly MappedConverter<TFrom, TTo> _converter;
        private ParameterExpression _fromParameter;
        private ParameterExpression _toParameter;
        private IMapper _mapper;

        public ExpressionConverter(IMapper mapper)
        {
            _mapper = mapper;
        }
        //public ExpressionConverter(MappedConverter<TFrom, TTo> converter)
        //{
        //    _converter = converter;
        //}

        public override Expression Visit(Expression node)
        {
            if (_fromParameter == null)
            {
                if (node.NodeType != ExpressionType.Lambda)
                {
                    throw new ArgumentException("Expression must be a lambda");
                }

                var lambda = (LambdaExpression)node;
                if (lambda.ReturnType != typeof(bool) || lambda.Parameters.Count != 1 ||
                    lambda.Parameters[0].Type != typeof(TFrom))
                {
                    throw new ArgumentException("Expression must be a Func<TFrom, bool>");
                }

                _fromParameter = lambda.Parameters[0];
                _toParameter = Expression.Parameter(typeof(TTo), _fromParameter.Name);
            }
            return base.Visit(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_fromParameter == node)
            {
                return _toParameter;
            }
            return base.VisitParameter(node);
        }


        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == _fromParameter)
            {
                var member = node.Member.Name;

                var typeMap = _mapper.ConfigurationProvider.ResolveTypeMap(typeof(TFrom), typeof(TTo));

                var propertyMaps = typeMap.GetPropertyMaps();

                var propertyMap = propertyMaps
                    .Where(s => s.SourceMember == node.Member)
                    .FirstOrDefault();

                if (propertyMap != null)
                {
                    member = propertyMap.DestinationProperty.Name;
                }

                #region 自定义特性方式
                //var attribute = (CustomSourceMemeberAttribute)typeof(TFrom).GetProperty(member)
                //    .GetCustomAttribute(typeof(CustomSourceMemeberAttribute));

                //if (attribute != null)
                //{
                //    member = attribute.Name;
                //}
                #endregion

                return Expression.Property(_toParameter, member);
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (typeof(T) == typeof(Func<TFrom, bool>))
            {
                return Expression.Lambda<Func<TTo, bool>>(Visit(node.Body), new[] { _toParameter });
            }
            return base.VisitLambda(node);
        }
    }



}


