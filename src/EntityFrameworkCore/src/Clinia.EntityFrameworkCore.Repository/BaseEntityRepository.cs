using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Clinia.EntityFrameworkCore.Repository
{
    public abstract class BaseEntityRepository<TEntity, TId>
    {
        private readonly DbContext _context;

        protected BaseEntityRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}