using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class MatriculaInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _logradouroRepo;
    private readonly AlunoRepository _alunoRepo;
    private readonly MatriculaRepository _matriculaRepo;

    public MatriculaInfrastructureTests()
    {
        _logradouroRepo = new LogradouroRepository(ConnectionString, DatabaseType);
        _alunoRepo = new AlunoRepository(ConnectionString, DatabaseType);
        _matriculaRepo = new MatriculaRepository(ConnectionString, DatabaseType);
    }

    private async Task<Matricula> CriarEInserirMatriculaAsync(
        MatriculaPlano plano = MatriculaPlano.Mensal,
        DateOnly? inicio = null,
        DateOnly? fim = null,
        MatriculaRestricoes restricoes = MatriculaRestricoes.None,
        string observacoes = "")
    {
        var aluno = await AlunoInfrastructureTests
            .CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        DateOnly dataInicio = inicio ?? DateOnly.FromDateTime(DateTime.Today);
        DateOnly dataFim = fim ?? dataInicio.AddMonths(1);
        Arquivo? laudo = restricoes == MatriculaRestricoes.None
            ? null
            : Arquivo.Criar(new byte[] { 1, 2, 3, 4 }).Value!;

        var result = Matricula.Criar(
            id: 0,
            aluno: aluno,
            plano: plano,
            dataInicio: dataInicio,
            dataFim: dataFim,
            objetivo: "Condicionamento Físico",
            restricoes: restricoes,
            observacoes: observacoes,
            laudo: laudo);

        if (result.IsFailure)
            throw new Exception(string.Join(", ", result.Notifications.Select(n => n.Mensagem)));

        return await _matriculaRepo.Adicionar(result.Value!);
    }

    [Fact]
    public async Task Matricula_Adicionar_E_ObterPorId_Sucesso()
    {
        var matricula = await CriarEInserirMatriculaAsync(
            MatriculaPlano.Mensal,
            restricoes: MatriculaRestricoes.Diabetes | MatriculaRestricoes.Alergias,
            observacoes: "Observação de teste");

        var obtida = await _matriculaRepo.ObterPorId(matricula.Id);

        Assert.NotNull(obtida);
        Assert.Equal(matricula.Id, obtida.Id);
        Assert.Equal(MatriculaPlano.Mensal, obtida.Plano);
        Assert.Equal(matricula.AlunoMatricula.Id, obtida.AlunoMatricula.Id);
        Assert.Equal(MatriculaRestricoes.Diabetes | MatriculaRestricoes.Alergias, obtida.RestricoesMedicas);
        Assert.NotNull(obtida.LaudoMedico);
    }

    [Fact]
    public async Task Matricula_ObterPorId_Inexistente_RetornaNulo()
    {
        Assert.Null(await _matriculaRepo.ObterPorId(999999));
    }

    [Fact]
    public async Task Matricula_ObterTodos_RetornaRegistros()
    {
        await CriarEInserirMatriculaAsync();
        Assert.NotEmpty(await _matriculaRepo.ObterTodos());
    }

    [Fact]
    public async Task Matricula_Atualizar_AlteraRegistro()
    {
        var original = await CriarEInserirMatriculaAsync();
        var atualizado = Matricula.Criar(
            id: original.Id,
            aluno: original.AlunoMatricula,
            plano: MatriculaPlano.Anual,
            dataInicio: original.DataInicio,
            dataFim: original.DataFim,
            objetivo: "Ganho de Massa Muscular",
            restricoes: MatriculaRestricoes.PressaoAlta,
            observacoes: "Observação atualizada",
            laudo: Arquivo.Criar(new byte[] { 9, 8, 7 }).Value!).Value!;

        await _matriculaRepo.Atualizar(atualizado);
        var noBanco = await _matriculaRepo.ObterPorId(original.Id);

        Assert.NotNull(noBanco);
        Assert.Equal(MatriculaPlano.Anual, noBanco.Plano);
        Assert.Equal("Ganho de Massa Muscular", noBanco.Objetivo);
        Assert.Equal(MatriculaRestricoes.PressaoAlta, noBanco.RestricoesMedicas);
    }

    [Fact]
    public async Task Matricula_Atualizar_Inexistente_LancaExcecao()
    {
        var original = await CriarEInserirMatriculaAsync();
        var inexistente = Matricula.Criar(
            id: 999999,
            aluno: original.AlunoMatricula,
            plano: original.Plano,
            dataInicio: original.DataInicio,
            dataFim: original.DataFim,
            objetivo: "Registro inexistente",
            restricoes: MatriculaRestricoes.None,
            observacoes: "",
            laudo: null).Value!;

        var ex = await Assert.ThrowsAsync<InfrastructureException>(
            () => _matriculaRepo.Atualizar(inexistente));
        Assert.Equal("REGISTRO_MATRICULA_NAO_ENCONTRADO", ex.ErrorCode);
    }

    [Fact]
    public async Task Matricula_Remover_ExcluiRegistro()
    {
        var matricula = await CriarEInserirMatriculaAsync();
        Assert.True(await _matriculaRepo.Remover(matricula.Id));
        Assert.Null(await _matriculaRepo.ObterPorId(matricula.Id));
    }

    [Fact]
    public async Task Matricula_Remover_Inexistente_RetornaFalse()
    {
        Assert.False(await _matriculaRepo.Remover(999999));
    }

    [Fact]
    public async Task Matricula_ObterPorAluno_FiltraCorretamente()
    {
        var matricula = await CriarEInserirMatriculaAsync();
        var lista = await _matriculaRepo.ObterPorAluno(matricula.AlunoMatricula.Id);

        Assert.NotEmpty(lista);
        Assert.Contains(lista, item => item.Id == matricula.Id);
    }

    [Fact]
    public async Task Matricula_ObterPorAlunoAsync_CumpreInterface()
    {
        var matricula = await CriarEInserirMatriculaAsync();
        var lista = await _matriculaRepo.ObterPorAlunoAsync(matricula.AlunoMatricula.Id);

        Assert.Contains(lista, item => item.Id == matricula.Id);
    }

    [Fact]
    public async Task Matricula_Ativa_E_PossuiMatriculaAtiva()
    {
        var matricula = await CriarEInserirMatriculaAsync(
            inicio: DateOnly.FromDateTime(DateTime.Today),
            fim: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));

        Assert.True(await _matriculaRepo.PossuiMatriculaAtiva(matricula.AlunoMatricula.Id));
        var ativa = await _matriculaRepo.ObterMatriculaAtivaPorAluno(matricula.AlunoMatricula.Id);
        Assert.NotNull(ativa);
        Assert.Equal(matricula.Id, ativa.Id);
    }

    [Fact]
    public async Task Matricula_ObterAtivas_FiltraPorAluno()
    {
        var matricula = await CriarEInserirMatriculaAsync(
            inicio: DateOnly.FromDateTime(DateTime.Today),
            fim: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));

        var ativas = await _matriculaRepo.ObterAtivas(matricula.AlunoMatricula.Id);
        Assert.Contains(ativas, item => item.Id == matricula.Id);
    }

    [Fact]
    public async Task Matricula_ObterVencendoEmDias_RetornaProximasDoVencimento()
    {
        var matricula = await CriarEInserirMatriculaAsync(
            inicio: DateOnly.FromDateTime(DateTime.Today),
            fim: DateOnly.FromDateTime(DateTime.Today.AddDays(10)));

        var vencendo = await _matriculaRepo.ObterVencendoEmDias(30);
        Assert.Contains(vencendo, item => item.Id == matricula.Id);
    }

    [Fact]
    public async Task Matricula_ObterPorPlano_FiltraCorretamente()
    {
        var matricula = await CriarEInserirMatriculaAsync(MatriculaPlano.Trimestral);
        var lista = await _matriculaRepo.ObterPorPlano(MatriculaPlano.Trimestral);

        Assert.Contains(lista, item => item.Id == matricula.Id);
    }
}
