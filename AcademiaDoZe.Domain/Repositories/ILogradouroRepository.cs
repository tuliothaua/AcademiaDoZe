// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface ILogradouroRepository : IRepository<Logradouro>
{
    Task<Logradouro?> ObterPorCepAsync(
        string cep,
        CancellationToken cancellationToken = default);
}
