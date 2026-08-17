// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IAcessoAlunoRepository : IRepository<AcessoAluno>
{
    Task<IReadOnlyCollection<AcessoAluno>> ObterPorAlunoAsync(
        int alunoId,
        DateOnly data,
        CancellationToken cancellationToken = default);
}
