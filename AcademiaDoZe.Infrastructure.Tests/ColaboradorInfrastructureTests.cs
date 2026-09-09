using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class ColaboradorInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _logradouroRepo;
    private readonly ColaboradorRepository _colaboradorRepo;

    public ColaboradorInfrastructureTests()
    {
        _logradouroRepo = new LogradouroRepository(ConnectionString, DatabaseType);
        _colaboradorRepo = new ColaboradorRepository(ConnectionString, DatabaseType);
    }

    private async Task<Colaborador> CriarEInserirAsync(
        ColaboradorRepository colaboradorRepo, LogradouroRepository logradouroRepo)
    {
        var logradouro = await LogradouroInfrastructureTests.CriarEInserirLogradouroAsync(logradouroRepo, DatabaseType);
        var foto = Arquivo.Criar(new byte[] { 5, 6, 7, 8 }).Value!;
        var result = Colaborador.Criar(
            0,
            "THAUA",
            GerarCpf(),
            new DateOnly(1995, 5, 15),
            GerarTelefone(),
            GerarEmail(),
            "SQLite_Senha123",
            foto,
            logradouro,
            "200",
            "THAUA DUTRA",
            new DateOnly(2023, 1, 1),
            ColaboradorTipo.Instrutor,
            ColaboradorVinculo.CLT);

        if (result.IsFailure)
            throw new Exception(string.Join(", ", result.Notifications.Select(n => n.Mensagem)));

        return await colaboradorRepo.Adicionar(result.Value!);
    }

    [Fact]
    public async Task Adicionar_E_ObterPorId_Sucesso()
    {
        var colaborador = await CriarEInserirAsync(_colaboradorRepo, _logradouroRepo);
        var obtido = await _colaboradorRepo.ObterPorId(colaborador.Id);
        Assert.NotNull(obtido);
        Assert.Equal(colaborador.Id, obtido.Id);
        Assert.Equal(colaborador.Cpf.Valor, obtido.Cpf.Valor);
        Assert.Equal(colaborador.Email.Valor, obtido.Email.Valor);
    }

    [Fact]
    public async Task ObterPorId_Inexistente_RetornaNulo()
    {
        Assert.Null(await _colaboradorRepo.ObterPorId(999999));
    }

    [Fact]
    public async Task ObterTodos_RetornaRegistros()
    {
        await CriarEInserirAsync(_colaboradorRepo, _logradouroRepo);
        Assert.NotEmpty(await _colaboradorRepo.ObterTodos());
    }

    [Fact]
    public async Task Atualizar_AlteraRegistro()
    {
        var original = await CriarEInserirAsync(_colaboradorRepo, _logradouroRepo);
        var alterado = Colaborador.Criar(
            original.Id,
            "TULIO THAUA",
            original.Cpf.Valor,
            original.DataNascimento,
            original.Telefone.Valor,
            original.Email.Valor,
            original.Senha.Valor,
            original.Foto,
            original.Endereco,
            "300",
            "DUTRA",
            original.DataAdmissao,
            ColaboradorTipo.Administrador,
            original.Vinculo).Value!;

        await _colaboradorRepo.Atualizar(alterado);
        var noBanco = await _colaboradorRepo.ObterPorId(original.Id);
        Assert.Equal("TULIO THAUA", noBanco!.Nome);
        Assert.Equal(ColaboradorTipo.Administrador, noBanco.Tipo);
    }

    [Fact]
    public async Task Atualizar_Inexistente_LancaExcecao()
    {
        var logradouro = await LogradouroInfrastructureTests.CriarEInserirLogradouroAsync(_logradouroRepo, DatabaseType);
        var colaborador = Colaborador.Criar(
            999999,
            "INEXISTENTE",
            GerarCpf(),
            new DateOnly(1990, 1, 1),
            GerarTelefone(),
            GerarEmail(),
            "SQLite_Senha123",
            Arquivo.Criar(new byte[] { 1, 2 }).Value!,
            logradouro,
            "1",
            "DUTRA",
            new DateOnly(2020, 1, 1),
            ColaboradorTipo.Atendente,
            ColaboradorVinculo.CLT).Value!;

        var ex = await Assert.ThrowsAsync<InfrastructureException>(() => _colaboradorRepo.Atualizar(colaborador));
        Assert.Equal("REGISTRO_NAO_ENCONTRADO", ex.ErrorCode);
    }

    [Fact]
    public async Task Remover_ExcluiRegistro()
    {
        var colaborador = await CriarEInserirAsync(_colaboradorRepo, _logradouroRepo);
        Assert.True(await _colaboradorRepo.Remover(colaborador.Id));
        Assert.Null(await _colaboradorRepo.ObterPorId(colaborador.Id));
    }

    [Fact]
    public async Task Remover_Inexistente_RetornaFalse()
    {
        Assert.False(await _colaboradorRepo.Remover(999999));
    }

    [Fact]
    public async Task ObterPorCpf_E_Email_LocalizaRegistro()
    {
        var colaborador = await CriarEInserirAsync(_colaboradorRepo, _logradouroRepo);
        Assert.Equal(colaborador.Id, (await _colaboradorRepo.ObterPorCpf(colaborador.Cpf))!.Id);
        Assert.Equal(colaborador.Id, (await _colaboradorRepo.ObterPorEmail(colaborador.Email))!.Id);
    }

    [Fact]
    public async Task Cpf_E_Email_JaExistem_RespeitamIdAtual()
    {
        var colaborador = await CriarEInserirAsync(_colaboradorRepo, _logradouroRepo);
        Assert.True(await _colaboradorRepo.CpfJaExiste(colaborador.Cpf));
        Assert.False(await _colaboradorRepo.CpfJaExiste(colaborador.Cpf, colaborador.Id));
        Assert.True(await _colaboradorRepo.EmailJaExiste(colaborador.Email));
        Assert.False(await _colaboradorRepo.EmailJaExiste(colaborador.Email, colaborador.Id));
    }

    [Fact]
    public async Task FiltrosPorTipo_E_Vinculo_RetornamRegistro()
    {
        var colaborador = await CriarEInserirAsync(_colaboradorRepo, _logradouroRepo);
        Assert.Contains(await _colaboradorRepo.ObterPorTipo(colaborador.Tipo), c => c.Id == colaborador.Id);
        Assert.Contains(await _colaboradorRepo.ObterPorVinculo(colaborador.Vinculo), c => c.Id == colaborador.Id);
    }

    [Fact]
    public async Task TrocarSenha_AtualizaSenha()
    {
        var colaborador = await CriarEInserirAsync(_colaboradorRepo, _logradouroRepo);
        var novaSenha = Senha.Criar("NovaSenhaSQLite123").Value!;
        Assert.True(await _colaboradorRepo.TrocarSenha(colaborador.Id, novaSenha));
        Assert.Equal("NovaSenhaSQLite123", (await _colaboradorRepo.ObterPorId(colaborador.Id))!.Senha.Valor);
        Assert.False(await _colaboradorRepo.TrocarSenha(999999, novaSenha));
    }
}
