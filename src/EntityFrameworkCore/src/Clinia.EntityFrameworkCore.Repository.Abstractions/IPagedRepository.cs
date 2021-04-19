using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Clinia.EntityFrameworkCore.Repository
{
    public interface IPagedRepository<TEntity, TId> 
        where TEntity : class
        where TId : IEquatable<TId>
    {
        PagedResult<TEntity, TId> GetPaged(
            int page, 
            int size, 
            SortDirection sortDirection = SortDirection.Ascending);

        Task<PagedResult<TEntity, TId>> GetPagedAsync(
            int page, 
            int size, 
            SortDirection sortDirection = SortDirection.Ascending);
        
        PagedResult<TEntity, TId> GetPaged(
            int page, 
            int size, 
            string orderByPropertyName, 
            SortDirection sortDirection = SortDirection.Ascending);

        Task<PagedResult<TEntity, TId>> GetPagedAsync(
            int page, 
            int size, 
            string orderByPropertyName, 
            SortDirection sortDirection = SortDirection.Ascending);

        PagedResult<TEntity, TId> GetPaged(
            int page, 
            int size, 
            string orderByPropertyName, 
            SortDirection sortDirection = SortDirection.Ascending, 
            params Expression<Func<TEntity, object>>[] includes);

        Task<PagedResult<TEntity, TId>> GetPagedAsync(
            int page, 
            int size, 
            string orderByPropertyName, 
            SortDirection sortDirection = SortDirection.Ascending, 
            params Expression<Func<TEntity, object>>[] includes);

        PagedResult<TEntity, TId> GetPaged(
            int page, 
            int size, 
            string orderByPropertyName,
            SortDirection sortDirection,
            Expression<Func<TEntity, bool>> predicate);

        Task<PagedResult<TEntity, TId>> GetPagedAsync(
            int page, 
            int size, 
            string orderByPropertyName,
            SortDirection sortDirection,
            Expression<Func<TEntity, bool>> predicate);

        PagedResult<TEntity, TId> GetPaged(
            int page, 
            int size, 
            string orderByPropertyName, 
            SortDirection sortDirection,
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        Task<PagedResult<TEntity, TId>> GetPagedAsync(
            int page, 
            int size, 
            string orderByPropertyName,
            SortDirection sortDirection,
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);
    }
}