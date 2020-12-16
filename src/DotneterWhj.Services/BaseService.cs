using AutoMapper;
using AutoMapper.QueryableExtensions;
using DotneterWhj.DataTransferObject;
using DotneterWhj.EntityModel;
using DotneterWhj.IServices;
using DotneterWhj.Repository;
using DotneterWhj.Repository.UnitOfWork;
using DotneterWhj.ToolKits;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Services
{
    public abstract class BaseService<TDto, TEntity> : IBaseService<TDto> where TDto : class, new()
                                                                          where TEntity : BaseEntity, new()
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IMapper Mapper;

        public BaseService(IRepository<TEntity> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            Repository = repository;
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task<IQueryable<TDto>> GetAllAsync()
        {
            var entities = await Repository.QueryAsync();

            Type entityType = typeof(TEntity);

            Type dtoType = typeof(TDto);

            List<TDto> dtos = new List<TDto>();

            foreach (var e in entities)
            {
                object dto = Activator.CreateInstance(dtoType);

                foreach (var item in dtoType.GetProperties())
                {
                    item.SetValue(dto, entityType.GetProperty(item.Name).GetValue(e));
                }

                dtos.Add((TDto)dto);
            }

            return dtos.AsQueryable();
        }

        public async Task<bool> IsExist(object objId)
        {
            return await Repository.IsExist(objId);
        }

        public async Task<TDto> InsertAsync(TDto tDto)
        {
            var entity = Mapper.Map<TEntity>(tDto);

            return Mapper.Map<TDto>(await Repository.AddAsync(entity));
        }

        public async Task<TDto> AddAsync(TDto tDto)
        {
            var entity = Mapper.Map<TEntity>(tDto);

            return Mapper.Map<TDto>(await Repository.AddAsync(entity));
        }

        public async Task<int> AddAsync(List<TDto> lisTDto)
        {
            var entities = Mapper.Map<List<TEntity>>(lisTDto);

            return await Repository.AddAsync(entities);
        }

        public async Task<int> DeleteAsync(TDto tDto)
        {
            var entity = Mapper.Map<TEntity>(tDto);

            return await Repository.DeleteAsync(entity);
        }

        public async Task<int> DeleteAsync(List<TDto> tDtos)
        {
            var entities = Mapper.Map<List<TEntity>>(tDtos);

            return await Repository.DeleteAsync(entities);
        }

        public async Task<int> DeleteByIdAsync(object id)
        {
            return await Repository.DeleteByIdAsync(id);
        }

        public async Task<int> UpdateAsync(TDto tDto)
        {
            return await DynamicUpdateAsync(tDto);
        }

        public async Task<int> UpdateWithFiledsAsync(TDto tDto, params string[] fileds)
        {
            if (fileds == null)
            {
                return await UpdateAsync(tDto);
            }

            TEntity entity = Mapper.Map<TEntity>(tDto);

            return await Repository.UpdateAsync(entity, fileds);
        }

        public async Task<int> UpdateAsync(TDto tDto, params string[] filedsNotUpdate)
        {
            return await DynamicUpdateAsync(tDto, filedsNotUpdate);
        }


        public async Task<int> UpdateAsync(TDto[] tDtos)
        {
            if (tDtos == null) throw new ArgumentNullException();

            int result = 0;

            foreach (var item in tDtos)
            {
                result += await UpdateAsync(item);
            }

            return result;
        }

        private async Task<int> DynamicUpdateAsync(TDto tDto)
        {
            // 序列化动态Json为字符串
            string json = JsonConvert.SerializeObject(tDto);

            // 反序列化为数据表中的实体对象
            TEntity entity = Mapper.Map<TEntity>(tDto);

            // 反序列化为动态对象中的属性
            var jsonModel = JsonConvert.DeserializeObject<dynamic>(json);

            // 定义一个List来添加属性
            List<string> listName = new List<string>();

            // 动态添加要修改的字段
            foreach (PropertyInfo info in entity.GetType().GetProperties())
            {
                // 如果EF表中有实体对象，则排除，否则更新会报错,保留枚举
                if ((info.PropertyType.IsClass && info.PropertyType == typeof(string)) || info.PropertyType.IsClass == false)
                {
                    // 解决大小写问题
                    foreach (var property in jsonModel)
                    {
                        PropertyInfo modelProperty = tDto.GetType().GetProperty(property.Name);
                        var value = modelProperty.GetValue(tDto);

                        if (info.Name.ToLower().Trim() == property.Name.ToLower().Trim()
                           && value != null)
                        {
                            listName.Add(info.Name);
                        }
                    }
                }
            }

            return await Repository.UpdateAsync(entity, listName.ToArray());
        }


        private async Task<int> DynamicUpdateAsync(TDto tDto, string[] fieldsNotUpdate)
        {
            // 序列化动态Json为字符串
            string json = JsonConvert.SerializeObject(tDto);

            // 反序列化为数据表中的实体对象
            TEntity entity = Mapper.Map<TEntity>(tDto);

            // 反序列化为动态对象中的属性
            var jsonModel = JsonConvert.DeserializeObject<dynamic>(json);

            // 定义一个List来添加属性
            List<string> listName = new List<string>();

            // 动态添加要修改的字段
            foreach (PropertyInfo info in entity.GetType().GetProperties())
            {
                // 如果EF表中有实体对象，则排除，否则更新会报错,保留枚举
                if ((info.PropertyType.IsClass && info.PropertyType == typeof(string)) || info.PropertyType.IsClass == false)
                {
                    // 解决大小写问题
                    foreach (var property in jsonModel)
                    {
                        PropertyInfo modelProperty = tDto.GetType().GetProperty(property.Name);
                        var value = modelProperty.GetValue(tDto);

                        if (info.Name.ToLower().Trim() == property.Name.ToLower().Trim()
                           && value != null)
                        {
                            listName.Add(info.Name);
                        }
                    }
                }
            }

            if (fieldsNotUpdate != null)
            {
                foreach (var filed in fieldsNotUpdate)
                {
                    if (listName.Contains(filed)) listName.Remove(filed);
                }
            }

            return await Repository.UpdateAsync(entity, listName.ToArray());
        }

        /// <summary>
        /// 更新指定实体,不更新指定字段,如果每个表中有相同不更新的字段，可以这样写
        /// </summary>
        /// <typeparam name="T">数据表实体Model模型</typeparam>
        /// <param name="modelNew">动态Json数据</param>
        protected virtual void UpdateSpecify<T>(dynamic modelNew)
        {
            ////序列化动态Json为字符串
            //string json = modelNew.ToString();

            ////反序列化为数据表中的实体对象
            //T model = JsonConvert.DeserializeObject<T>(json);

            ////反序列化为动态对象中的属性
            //var jsonModel = JsonConvert.DeserializeObject<dynamic>(json);

            ////定义一个List来添加属性
            //List<string> listName = new List<string>();

            ////定义不需要更新的字段
            //string fieldProNames = "field1,field2,field3,CreateDate,Creator,IsDel,Updator,UpdateDate";

            ////动态添加要修改的字段
            //foreach (PropertyInfo info in model.GetType().GetProperties())
            //{
            //    //如果EF表中有实体对象，则排除，否则更新会报错,保留枚举
            //    if ((info.PropertyType.IsClass && info.PropertyType == typeof(String)) || info.PropertyType.IsClass == false)
            //    {
            //        //解决大小写问题
            //        foreach (var property in jsonModel && !fieldProNames.Split(",").Select(n => n.ToLower()).Contains(info.Name.ToLower()))
            //        {
            //            if (info.Name.ToLower().Trim() == property.Name.ToLower().Trim())
            //            {
            //                listName.Add(info.Name);
            //            }
            //        }
            //    }
            //}//寻找主键
            //PropertyInfo pkProp = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0).FirstOrDefault();

            ////遍历修改,并排除主键
            //foreach (string Name in listName)
            //{
            //    if (Name.ToLower() != pkProp.Name.ToLower())
            //    {
            //        _context.Entry(model).Property(Name).IsModified = true;
            //    }
            //}

            //return db.SaveChanges();
        }

        /// <summary>
        /// 更新指定实体,不更新指定字段,如果每个表中有相同不更新的字段，可以这样写,扩展方法
        /// </summary>
        /// <typeparam name="T">数据表实体Model模型</typeparam>
        /// <param name="modelNew">动态Json数据</param>
        /// <param name="fieldProNames">不更新的字段列表数组</param>
        protected virtual void UpdateSpecify<T>(dynamic modelNew, string fieldProNames)
        {
            ////序列化动态Json为字符串
            //string json = modelNew.ToString();

            ////反序列化为数据表中的实体对象
            //T model = JsonConvert.DeserializeObject<T>(json);

            ////把状态全部变为不可更改
            //_context.Entry(model).State = EntityState.Unchanged;

            ////反序列化为动态对象中的属性
            //var jsonModel = JsonConvert.DeserializeObject<dynamic>(json);

            ////定义一个List来添加属性
            //List<string> listName = new List<string>();

            ////定义不需要更新的字段
            //string fieldProNameses = "field1,field2,field3,CreateDate,Creator,IsDel,Updator,UpdateDate," + fieldProNames;

            ////动态添加要修改的字段
            //foreach (PropertyInfo info in model.GetType().GetProperties())
            //{
            //    //如果EF表中有实体对象，则排除，否则更新会报错,保留枚举
            //    if ((info.PropertyType.IsClass && info.PropertyType == typeof(String)) || info.PropertyType.IsClass == false)
            //    {
            //        //解决大小写问题
            //        foreach (var property in jsonModel && !fieldProNameses.Split(",").Select(n => n.ToLower()).Contains(info.Name.ToLower()))
            //        {
            //            if (info.Name.ToLower().Trim() == property.Name.ToLower().Trim())
            //            {
            //                listName.Add(info.Name);
            //            }
            //        }
            //    }
            //}

            ////寻找主键
            //PropertyInfo pkProp = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0).FirstOrDefault();

            ////遍历修改,并排除主键
            //foreach (string Name in listName)
            //{
            //    if (Name.ToLower() != pkProp.Name.ToLower())
            //    {
            //        _context.Entry(model).Property(Name).IsModified = true;
            //    }
            //}

            ////return db.SaveChanges();
        }


        public async Task<IEnumerable<TDto>> QueryAsync()
        {
            var tEntities = await Repository.QueryAsync();

            return Mapper.Map<IEnumerable<TDto>>(tEntities.AsEnumerable());
        }

        private void CheckNull(string[] fields)
        {
            if (fields == null || fields.Length <= 0)
            {
                throw new ArgumentNullException(nameof(fields));
            }
        }

        public async Task<IEnumerable<TDto>> QueryAsync(string[] fields)
        {
            CheckNull(fields);

            var tEntities = await Repository.QueryAsync();

            var typeMap = Mapper.ConfigurationProvider.ResolveTypeMap(typeof(TDto), typeof(TEntity));

            var selector = ExpressionSelect.CreateSelecter<TEntity, TDto>(typeMap, fields.ToList());

            return tEntities.Select(selector).AsEnumerable();

            // return Mapper.Map<IEnumerable<TDto>>(tEntities.AsEnumerable());
        }

        public async Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression)
        {
            //var whereExp = _mapper.Map<Expression<Func<TEntity, bool>>>(whereExpression);

            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var tEntities = await Repository.QueryAsync(newExperssion);

            return Mapper.Map<IEnumerable<TDto>>(tEntities.AsEnumerable());
        }

        public async Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, string[] fields)
        {
            CheckNull(fields);

            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var tEntities = await Repository.QueryAsync(newExperssion);

            var typeMap = Mapper.ConfigurationProvider.ResolveTypeMap(typeof(TDto), typeof(TEntity));

            var selector = ExpressionSelect.CreateSelecter<TEntity, TDto>(typeMap, fields.ToList());

            return tEntities.Select(selector).AsEnumerable();
        }

        public async Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, params OrderFiledModel[] orderByFileds)
        {
            orderByFileds = PropertyConvert(orderByFileds);

            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var tEntities = await Repository.QueryAsync(newExperssion, orderByFileds);

            return Mapper.Map<IEnumerable<TDto>>(tEntities.AsEnumerable());
        }

        public async Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, string[] fields, params OrderFiledModel[] orderByFileds)
        {
            CheckNull(fields);

            orderByFileds = PropertyConvert(orderByFileds);

            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var tEntities = await Repository.QueryAsync(newExperssion, orderByFileds);

            var typeMap = Mapper.ConfigurationProvider.ResolveTypeMap(typeof(TDto), typeof(TEntity));

            var selector = ExpressionSelect.CreateSelecter<TEntity, TDto>(typeMap, fields.ToList());

            return tEntities.Select(selector).AsEnumerable();
        }

        public async Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, Expression<Func<TDto, object>> orderByExpression, bool isAsc = true)
        {
            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var orderByExp = Mapper.Map<Expression<Func<TEntity, object>>>(orderByExpression);

            var tEntities = await Repository.QueryAsync(newExperssion, orderByExp, isAsc);

            return Mapper.Map<IEnumerable<TDto>>(tEntities.AsEnumerable());
        }

        public async Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, Expression<Func<TDto, object>> orderByExpression, string[] fields, bool isAsc = true)
        {
            CheckNull(fields);

            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var orderByExp = Mapper.Map<Expression<Func<TEntity, object>>>(orderByExpression);

            var tEntities = await Repository.QueryAsync(newExperssion, orderByExp, isAsc);

            var typeMap = Mapper.ConfigurationProvider.ResolveTypeMap(typeof(TDto), typeof(TEntity));

            var selector = ExpressionSelect.CreateSelecter<TEntity, TDto>(typeMap, fields.ToList());

            return tEntities.Select(selector).AsEnumerable();
        }


        public async Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, int intTop, params OrderFiledModel[] orderByFileds)
        {
            orderByFileds = PropertyConvert(orderByFileds);

            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var tEntities = await Repository.QueryAsync(newExperssion, intTop, orderByFileds);

            return Mapper.Map<IEnumerable<TDto>>(tEntities.AsEnumerable());
        }

        public async Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, int intTop, string[] fields, params OrderFiledModel[] orderByFileds)
        {
            CheckNull(fields);

            orderByFileds = PropertyConvert(orderByFileds);

            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var tEntities = await Repository.QueryAsync(newExperssion, intTop, orderByFileds);

            var typeMap = Mapper.ConfigurationProvider.ResolveTypeMap(typeof(TDto), typeof(TEntity));

            var selector = ExpressionSelect.CreateSelecter<TEntity, TDto>(typeMap, fields.ToList());

            return tEntities.Select(selector).AsEnumerable();
        }

        public async Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, int intPageIndex, int intPageSize, params OrderFiledModel[] orderByFileds)
        {
            orderByFileds = PropertyConvert(orderByFileds);

            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var tEntities = await Repository.QueryAsync(newExperssion, intPageIndex, intPageSize, orderByFileds);

            return Mapper.Map<IEnumerable<TDto>>(tEntities.AsEnumerable());
        }


        public async Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, int intPageIndex, int intPageSize, string[] fields, params OrderFiledModel[] orderByFileds)
        {
            CheckNull(fields);

            orderByFileds = PropertyConvert(orderByFileds);

            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var tEntities = await Repository.QueryAsync(newExperssion, intPageIndex, intPageSize, orderByFileds);

            var typeMap = Mapper.ConfigurationProvider.ResolveTypeMap(typeof(TDto), typeof(TEntity));

            var selector = ExpressionSelect.CreateSelecter<TEntity, TDto>(typeMap, fields.ToList());

            return tEntities.Select(selector).AsEnumerable();
        }

        public async Task<TDto> QueryByIdAsync(object objId)
        {
            var entity = await Repository.QueryByIdAsync(objId);

            return Mapper.Map<TDto>(entity);
        }


        public async Task<IEnumerable<TDto>> QueryByIDsAsync(object[] lstIds)
        {
            var entities = await Repository.QueryByIDsAsync(lstIds);

            return Mapper.Map<List<TDto>>(entities.AsEnumerable());
        }

        public async Task<TDto> QueryByKeyAsync(params object[] objId)
        {
            var entity = await Repository.QueryByKeyAsync(objId);

            return Mapper.Map<TDto>(entity);
        }

        public async Task<List<TResult>> QueryMuchAsync<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task<PageModel<TDto>> QueryPageAsync(Expression<Func<TDto, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, params OrderFiledModel[] orderByFileds)
        {
            orderByFileds = PropertyConvert(orderByFileds);

            ExpressionConverter<TDto, TEntity> expressionConverter = new ExpressionConverter<TDto, TEntity>(Mapper);

            var newExperssion = (Expression<Func<TEntity, bool>>)expressionConverter.Visit(whereExpression);

            var entities = await Repository.QueryPageAsync(newExperssion, intPageIndex, intPageSize, orderByFileds);

            return Mapper.Map<PageModel<TDto>>(entities);
        }

        public async Task<IEnumerable<TDto>> QuerySqlAsync(string strSql, SqlParameter[] parameters = null)
        {
            throw new NotImplementedException();
        }

        private OrderFiledModel[] PropertyConvert(OrderFiledModel[] orderFiledModels)
        {
            if (orderFiledModels == null)
            {
                return orderFiledModels;
            }

            var typeMap = Mapper.ConfigurationProvider.ResolveTypeMap(typeof(TDto), typeof(TEntity));

            var propertyMaps = typeMap.GetPropertyMaps();

            foreach (var orderFiled in orderFiledModels)
            {
                var dtoMemberName = orderFiled.PropertyName;

                var propertyMap = propertyMaps
                    .Where(s => s.SourceMember.Name.Equals(dtoMemberName, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                if (propertyMap != null)
                {
                    dtoMemberName = propertyMap.DestinationProperty.Name;
                }

                orderFiled.PropertyName = dtoMemberName;
            }

            return orderFiledModels;
        }


    }
}
