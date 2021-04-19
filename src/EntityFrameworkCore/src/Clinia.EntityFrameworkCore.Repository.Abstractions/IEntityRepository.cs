using System;

namespace Clinia.EntityFrameworkCore.Repository
{
    public interface IEntityRepository<TEntity, TId>
        where TEntity : class
        where TId : IEquatable<TId>
    {
        
    }
}