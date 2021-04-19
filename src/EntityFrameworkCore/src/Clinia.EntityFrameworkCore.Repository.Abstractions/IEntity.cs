using System;

namespace Clinia.EntityFrameworkCore.Repository
{
    public interface IEntity<out TId> where TId : IEquatable<TId>
    {
        TId Id { get; }
    }
}