using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Clinia.EntityFrameworkCore.Repository
{
    public interface IReadEntityRepository<TEntity, TId> 
        where TEntity : class
        where TId : IEquatable<TId>
    {
        /// <summary>
        ///     Returns all entities
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        ///     Get all entities while only returning the specified properties defined in the project.
        ///     Can be useful when only wanting to access a limited amount of properties for all entities. 
        /// </summary>
        /// <param name="projectToFunc"></param>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        IEnumerable<TType> GetAll<TType>(Expression<Func<TEntity, TType>> projectToFunc) where TType : class;

        /// <summary>
        ///     Returns all entities
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllAsync();
        
        /// <summary>
        ///     Get all entities while only returning the specified properties defined in the project.
        ///     Can be useful when only wanting to access a limited amount of properties for all entities.
        /// </summary>
        /// <param name="projectToFunc"></param>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        Task<IEnumerable<TType>> GetAllAsync<TType>(Expression<Func<TEntity, TType>> projectToFunc) where TType : class;
        
        /// <summary>
        ///     Get all entities with their relationships specified
        /// </summary>
        /// <param name="includes"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes);
        
        IEnumerable<TType> GetAll<TType>(Expression<Func<TEntity, TType>> projectToFunc,
            params Expression<Func<TEntity, object>>[] includes) where TType : class;
        
        Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TType>> GetAllAsync<TType>(Expression<Func<TEntity, TType>> projectToFunc,
            params Expression<Func<TEntity, object>>[] includes) where TType : class;
        
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TType> FindAll<TType>(Expression<Func<TEntity, TType>> projectToFunc,
            Expression<Func<TEntity, bool>> predicate) where TType : class;

        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TType>> FindAllAsync<TType>(Expression<Func<TEntity, TType>> projectToFunc,
            Expression<Func<TEntity, bool>> predicate) where TType : class;

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        IEnumerable<TType> FindAll<TType>(Expression<Func<TEntity, TType>> projectToFunc, Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes) where TType : class;

        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TType>> FindAllAsync<TType>(Expression<Func<TEntity, TType>> projectToFunc,
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes) where TType : class;
        
        TEntity Find(TId key);

        Task<TEntity> FindAsync(TId key);

        TEntity Find(TId key, params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> FindAsync(TId key, params Expression<Func<TEntity, object>>[] includes);

        TEntity Find(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Find(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        bool Exists(Expression<Func<TEntity, bool>> predicate);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    }
}