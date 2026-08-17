// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IMatriculaRepository : IRepository<Matricula>
{
    Task<IReadOnlyCollection<Matricula>> ObterPorAlunoAsync(
        int alunoId,
        CancellationToken cancellationToken = default);
}
