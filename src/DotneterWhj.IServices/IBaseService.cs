using DotneterWhj.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DotneterWhj.ToolKits;

namespace DotneterWhj.IServices
{
    public interface IBaseService<TDto> where TDto : class, new()
        //where TEntity : class, new()
    {
        Task<IQueryable<TDto>> GetAllAsync();

        Task<TDto> InsertAsync(TDto tDto);

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="tDto">待新增的实体</param>
        /// <returns></returns>
        Task<TDto> AddAsync(TDto tDto);

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="lisTDto">待新增的实体集合</param>
        /// <returns></returns>
        Task<int> AddAsync(List<TDto> lisTDto);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteByIdAsync(object id);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="tDto"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(TDto tDto);

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="tDtos"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(List<TDto> tDtos);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tDto"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(TDto tDto);

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="tDtos"></param>
        /// <remarks>需要开启事务操作,否则可能会造成数据不一致</remarks>
        /// <returns></returns>
        Task<int> UpdateAsync(TDto[] tDtos);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tDto"></param>
        /// <param name="fileds">要更新的字段</param>
        /// <returns></returns>
        Task<int> UpdateWithFiledsAsync(TDto tDto, params string[] fileds);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tDto"></param>
        /// <param name="filedsNotUpdate">忽略的字段</param>
        /// <returns></returns>
        Task<int> UpdateAsync(TDto tDto, params string[] filedsNotUpdate);

        ///// <summary>
        ///// 动态更新数据
        ///// </summary>
        ///// <param name="modelNew"></param>
        ///// <returns></returns>
        //Task<int> DynamicUpdateAsync(object modelNew);

        ///// <summary>
        ///// 动态更新数据
        ///// </summary>
        ///// <param name="modelNew"></param>
        ///// <param name="fieldsNotUpdate">不需要更新的字段</param>
        ///// <returns></returns>
        //Task<int> DynamicUpdateAsync(dynamic modelNew, string[] fieldsNotUpdate);



        /// <summary>
        /// 是否存在数据
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task<bool> IsExist(object objId);

        /// <summary>
        /// 通过主键查询数据
        /// </summary>
        /// <param name="objId">主键</param>
        /// <returns></returns>
        Task<TDto> QueryByIdAsync(object objId);

        /// <summary>
        /// 通过组合主键查询数据
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task<TDto> QueryByKeyAsync(params object[] objId);

        /// <summary>
        /// 通过主键查询多条数据
        /// </summary>
        /// <param name="lstIds">主键数组</param>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryByIDsAsync(object[] lstIds);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync();

        /// <summary>
        /// 查询数据(按需查询)
        /// </summary>
        /// <param name="fields">需要查询的字段,必须在数据库中存在</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(string[] fields);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="fields">查询的字段</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, string[] fields);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="orderByFileds">排序字段</param>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="fields">查询的字段</param>
        /// <param name="orderByFileds">排序字段</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, string[] fields, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="isAsc">是否升序，默认true</param>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, Expression<Func<TDto, object>> orderByExpression, bool isAsc = true);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="fields">查询的字段</param>
        /// <param name="isAsc">是否升序，默认true</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, Expression<Func<TDto, object>> orderByExpression, string[] fields, bool isAsc = true);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">取前几项</param>
        /// <param name="orderByFileds">排序字段</param>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, int intTop, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">取前几项</param>
        /// <param name="fields">查询的字段</param>
        /// <param name="orderByFileds">排序字段</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, int intTop, string[] fields, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 按sql语句查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QuerySqlAsync(string strSql, SqlParameter[] parameters = null);

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">第几页</param>
        /// <param name="intPageSize">数量</param>
        /// <param name="orderByFileds">排序</param>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, int intPageIndex, int intPageSize, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">第几页</param>
        /// <param name="intPageSize"></</param>
        /// <param name="fields">查询的字段</param>
        /// <param name="orderByFileds">排序</param>
        /// <exception cref="ArgumentNullException">fields</exception>
        /// <exception cref="ParamterNotExistException">字段不存在异常</exception>
        /// <returns></returns>
        Task<IEnumerable<TDto>> QueryAsync(Expression<Func<TDto, bool>> whereExpression, int intPageIndex, int intPageSize, string[] fields, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">第几页，默认1</param>
        /// <param name="intPageSize">数量，默认20</param>
        /// <param name="orderByFileds">排序</param>
        /// <returns></returns>
        Task<PageModel<TDto>> QueryPageAsync(Expression<Func<TDto, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, params OrderFiledModel[] orderByFileds);

        Task<List<TResult>> QueryMuchAsync<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();
    }
}
