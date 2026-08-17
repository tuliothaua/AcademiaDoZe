// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IRepository<TEntity>
    where TEntity : Entity, IAggregateRoot
{
    Task<TEntity?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TEntity>> ObterTodosAsync(
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        TEntity entidade,
        CancellationToken cancellationToken = default);

    Task AtualizarAsync(
        TEntity entidade,
        CancellationToken cancellationToken = default);

    Task RemoverAsync(
        TEntity entidade,
        CancellationToken cancellationToken = default);
}
