using System;
using System.Collections.Generic;

namespace Clinia.EntityFrameworkCore.Repository
{
    public class PagedResult<TEntity, TId> 
        where TEntity : class
        where TId : IEquatable<TId>
    {
        public PagedResult(
            IEnumerable<TEntity> data,
            int page,
            int perPage,
            int total,
            string sortBy,
            SortDirection sortDirection)
        {
            NumPages = perPage == 0 ? 0 : (int) Math.Ceiling((decimal) total / perPage);
            Data = data;
            Page = page;
            PerPage = perPage;
            Total = total;
            SortBy = sortBy;
            SortDirection = sortDirection;
        }
        
        
        public int PerPage { get; }
        
        public int Page { get; }
        
        public int Total { get; }
        
        public int NumPages { get; }
        
        public string SortBy { get; }
        
        public SortDirection SortDirection { get; }
        
        public IEnumerable<TEntity> Data { get; }
    }
}