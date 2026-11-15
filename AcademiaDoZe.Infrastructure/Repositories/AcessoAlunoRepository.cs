using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Infrastructure.Repositories;

public sealed class AcessoAlunoRepository : AcessoRepositoryBase<AcessoAluno, Aluno>, IAcessoAlunoRepository
{
    public AcessoAlunoRepository(Func<(string ConnectionString, DatabaseType DatabaseType)> configurationProvider)
        : base(configurationProvider, 0,
            (id, token) => ObterAlunoAsync(configurationProvider, id, token),
            CriarAcesso, acesso => acesso.Aluno.Id)
    {
    }

    public Task<IReadOnlyCollection<AcessoAluno>> ObterPorAlunoAsync(int alunoId, DateOnly data, CancellationToken cancellationToken = default) =>
        ObterPorPessoaAsync(alunoId, data, cancellationToken);

    private static AcessoAluno CriarAcesso(int id, Aluno aluno, DateTime dataHora)
    {
        var result = AcessoAluno.Criar(id, aluno, dataHora);
        return result.IsSuccess ? result.Value! : throw new InvalidOperationException(string.Join("; ", result.Notifications.Select(n => n.Mensagem)));
    }

    private static async Task<Aluno?> ObterAlunoAsync(Func<(string ConnectionString, DatabaseType DatabaseType)> configurationProvider, int id, CancellationToken cancellationToken)
    {
        await using var repository = new AlunoRepository(configurationProvider);
        return await repository.ObterPorIdAsync(id, cancellationToken);
    }
}
