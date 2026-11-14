using Application.Common.Interfaces;
using System.Linq.Expressions;

namespace Application.Common.Interfaces;

/// <summary>
/// Generic persistence contract for aggregate repositories.
/// Owned by the Application layer; implemented by Infrastructure.
/// </summary>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Deferred query root for read-side composition.
    /// </summary>
    IQueryable<T> EntityQuery { get; }

    // Basic persistence
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);

    // Helpers for simple cases
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}
