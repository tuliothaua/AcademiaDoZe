using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class AlunoInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _logradouroRepo;
    private readonly AlunoRepository _alunoRepo;

    public AlunoInfrastructureTests()
    {
        _logradouroRepo = new LogradouroRepository(ConnectionString, DatabaseType);
        _alunoRepo = new AlunoRepository(ConnectionString, DatabaseType);
    }

    internal static async Task<Aluno> CriarEInserirAlunoAsync(
        AlunoRepository alunoRepo,
        LogradouroRepository logradouroRepo)
    {
        var logradouro = await LogradouroInfrastructureTests
            .CriarEInserirLogradouroAsync(logradouroRepo, AcademiaDoZe.Infrastructure.Data.DatabaseType.Sqlite);
        var foto = Arquivo.Criar(new byte[] { 10, 20, 30 }).Value!;

        var result = Aluno.Criar(
            id: 0,
            nome: "TULIO",
            cpfTexto: GerarCpf(),
            dataNascimento: new DateOnly(2000, 5, 15),
            telefoneTexto: GerarTelefone(),
            emailTexto: GerarEmail(),
            senhaTexto: "SQLite_Senha123",
            foto: foto,
            logradouro: logradouro,
            numero: "100",
            complemento: "DUTRA");

        if (result.IsFailure)
            throw new Exception(string.Join(", ", result.Notifications.Select(n => n.Mensagem)));

        return await alunoRepo.Adicionar(result.Value!);
    }

    [Fact]
    public async Task Aluno_Adicionar_E_ObterPorId_Sucesso()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var obtido = await _alunoRepo.ObterPorId(aluno.Id);

        Assert.NotNull(obtido);
        Assert.Equal(aluno.Id, obtido.Id);
        Assert.Equal(aluno.Cpf.Valor, obtido.Cpf.Valor);
        Assert.Equal(aluno.Nome, obtido.Nome);
        Assert.Equal(aluno.Email.Valor, obtido.Email.Valor);
    }

    [Fact]
    public async Task Aluno_ObterPorId_Inexistente_RetornaNulo()
    {
        Assert.Null(await _alunoRepo.ObterPorId(999999));
    }

    [Fact]
    public async Task Aluno_ObterTodos_RetornaRegistros()
    {
        await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        Assert.NotEmpty(await _alunoRepo.ObterTodos());
    }

    [Fact]
    public async Task Aluno_Atualizar_AlteraRegistro()
    {
        var original = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var alterado = Aluno.Criar(
            id: original.Id,
            nome: "NOME ALTERADO",
            cpfTexto: original.Cpf.Valor,
            dataNascimento: original.DataNascimento,
            telefoneTexto: original.Telefone.Numero,
            emailTexto: original.Email.Endereco,
            senhaTexto: original.Senha.Valor,
            foto: original.Foto,
            logradouro: original.Logradouro,
            numero: "200",
            complemento: "SOBRENOME ALTERADO").Value!;

        await _alunoRepo.Atualizar(alterado);
        var noBanco = await _alunoRepo.ObterPorId(original.Id);

        Assert.NotNull(noBanco);
        Assert.Equal("NOME ALTERADO", noBanco.Nome);
        Assert.Equal("200", noBanco.Numero);
    }

    [Fact]
    public async Task Aluno_Atualizar_Inexistente_LancaExcecao()
    {
        var logradouro = await LogradouroInfrastructureTests
            .CriarEInserirLogradouroAsync(_logradouroRepo, AcademiaDoZe.Infrastructure.Data.DatabaseType.Sqlite);
        var foto = Arquivo.Criar(new byte[] { 1, 2 }).Value!;
        var aluno = Aluno.Criar(
            id: 999999,
            nome: "INEXISTENTE",
            cpfTexto: GerarCpf(),
            dataNascimento: new DateOnly(2000, 1, 1),
            telefoneTexto: GerarTelefone(),
            emailTexto: GerarEmail(),
            senhaTexto: "SQLite_Senha123",
            foto: foto,
            logradouro: logradouro,
            numero: "1",
            complemento: "SOBRENOME").Value!;

        var ex = await Assert.ThrowsAsync<InfrastructureException>(
            () => _alunoRepo.Atualizar(aluno));
        Assert.Equal("REGISTRO_ALUNO_NAO_ENCONTRADO", ex.ErrorCode);
    }

    [Fact]
    public async Task Aluno_Remover_ExcluiRegistro()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        Assert.True(await _alunoRepo.Remover(aluno.Id));
        Assert.Null(await _alunoRepo.ObterPorId(aluno.Id));
    }

    [Fact]
    public async Task Aluno_Remover_Inexistente_RetornaFalse()
    {
        Assert.False(await _alunoRepo.Remover(999999));
    }

    [Fact]
    public async Task Aluno_ObterPorCpf_LocalizaRegistro()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var obtido = await _alunoRepo.ObterPorCpf(aluno.Cpf);

        Assert.NotNull(obtido);
        Assert.Equal(aluno.Id, obtido.Id);
    }

    [Fact]
    public async Task Aluno_ObterPorCpfAsync_ComString_LocalizaRegistro()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var obtido = await _alunoRepo.ObterPorCpfAsync(aluno.Cpf.Valor);

        Assert.NotNull(obtido);
        Assert.Equal(aluno.Id, obtido.Id);
    }
}
