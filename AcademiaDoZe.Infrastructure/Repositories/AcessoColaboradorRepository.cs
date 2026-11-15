using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Infrastructure.Repositories;

public sealed class AcessoColaboradorRepository : AcessoRepositoryBase<AcessoColaborador, Colaborador>, IAcessoColaboradorRepository
{
    public AcessoColaboradorRepository(Func<(string ConnectionString, DatabaseType DatabaseType)> configurationProvider)
        : base(configurationProvider, 1,
            (id, token) => ObterColaboradorAsync(configurationProvider, id, token),
            CriarAcesso, acesso => acesso.Colaborador.Id)
    {
    }

    public Task<IReadOnlyCollection<AcessoColaborador>> ObterPorColaboradorAsync(int colaboradorId, DateOnly data, CancellationToken cancellationToken = default) =>
        ObterPorPessoaAsync(colaboradorId, data, cancellationToken);

    private static AcessoColaborador CriarAcesso(int id, Colaborador colaborador, DateTime dataHora)
    {
        var result = AcessoColaborador.Criar(id, colaborador, dataHora);
        return result.IsSuccess ? result.Value! : throw new InvalidOperationException(string.Join("; ", result.Notifications.Select(n => n.Mensagem)));
    }

    private static async Task<Colaborador?> ObterColaboradorAsync(Func<(string ConnectionString, DatabaseType DatabaseType)> configurationProvider, int id, CancellationToken cancellationToken)
    {
        await using var repository = new ColaboradorRepository(configurationProvider);
        return await repository.ObterPorIdAsync(id, cancellationToken);
    }
}
