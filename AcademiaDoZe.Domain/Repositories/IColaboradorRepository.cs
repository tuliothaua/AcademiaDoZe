// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IColaboradorRepository : IRepository<Colaborador>
{
    Task<Colaborador?> ObterPorCpfAsync(
        string cpf,
        CancellationToken cancellationToken = default);
}
