// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IAcessoColaboradorRepository : IRepository<AcessoColaborador>
{
    Task<IReadOnlyCollection<AcessoColaborador>> ObterPorColaboradorAsync(
        int colaboradorId,
        DateOnly data,
        CancellationToken cancellationToken = default);
}
