using DotneterWhj.DataTransferObject;
using DotneterWhj.EntityModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Repository
{
    public interface IRepository { }

    public interface IRepository<TEntity> : IRepository where TEntity : BaseEntity, new()
    {
        /// <summary>
        /// 是否存在数据
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task<bool> IsExist(object objId);

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="entity">待新增的实体</param>
        /// <returns></returns>
        Task<TEntity> AddAsync(TEntity entity);

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="listEntity">待新增的实体集合</param>
        /// <returns></returns>
        Task<int> AddAsync(List<TEntity> listEntity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteByIdAsync(object id);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<int> DeleteByUnionKeyAsync(params object[] keys);


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(TEntity entity);

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="entities"></param>
        /// <remarks>注意需要开启事务操作</remarks>
        /// <returns></returns>
        Task<int> DeleteAsync(List<TEntity> entities);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <remarks>注意: 该方法会更新除了主键之外的所有字段,要实现按需更新请传递参数</remarks>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity entity);

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="fileds">要更新的字段</param>
        /// <remarks>注意需要开启事务操作</remarks>
        /// <returns></returns>
        Task<int> UpdateAsync(List<TEntity> entities, params string[] fileds);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fileds">要更新的字段,不传递将更新除主键外的所有字段</param>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity entity, params string[] fileds);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="filedsNotUpdate">不需要更新的字段</param>
        /// <returns></returns>
        Task<int> UpdateWithFiledsNotAsync(TEntity entity, params string[] filedsNotUpdate);

        /// <summary>
        /// 通过主键查询数据
        /// </summary>
        /// <param name="objId">主键</param>
        /// <returns></returns>
        Task<TEntity> QueryByIdAsync(object objId);

        /// <summary>
        /// 通过组合主键查询数据
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task<TEntity> QueryByKeyAsync(params object[] objId);

        /// <summary>
        /// 通过主键查询数据集合
        /// </summary>
        /// <param name="lstIds">主键数组</param>
        /// <returns></returns>
        Task<List<TEntity>> QueryByIDsAsync(object[] lstIds);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<TEntity>> QueryAsync();

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="orderByFileds">排序字段</param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="isAsc">是否升序，默认true</param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);

        /// <summary>
        /// 按条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">取前几项</param>
        /// <param name="orderByFileds"></param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, int intTop, params OrderFiledModel[] orderByFileds);


        Task<IQueryable<TEntity>> QuerySqlAsync(string strSql, SqlParameter[] parameters = null);

        Task<int> QueryCountSqlAsync(string strSql, SqlParameter[] parameters = null);

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">第几页</param>
        /// <param name="intPageSize">数量</param>
        /// <param name="orderByFileds">排序</param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, params OrderFiledModel[] orderByFileds);

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">第几页，默认1</param>
        /// <param name="intPageSize">数量，默认20</param>
        /// <param name="orderByFileds">排序</param>
        /// <returns></returns>
        Task<PageModel<TEntity>> QueryPageAsync(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, params OrderFiledModel[] orderByFileds);

        Task<List<TResult>> QueryMuchAsync<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();
    }
}
