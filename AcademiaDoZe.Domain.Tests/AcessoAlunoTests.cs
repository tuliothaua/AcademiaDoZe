// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Entities;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class AcessoAlunoTests
{
    [Fact]
    public void DeveCriarAcessoValido()
    {
        var resultado = AcessoAluno.Criar(1, DomainTestData.Aluno(), DateTime.Now);
        Assert.True(resultado.IsSuccess);
    }

    [Fact]
    public void DeveGuardarAluno()
    {
        var resultado = AcessoAluno.Criar(1, DomainTestData.Aluno(), DateTime.Now);
        Assert.Equal(DomainTestData.Aluno().Id, resultado.Value!.Aluno.Id);
    }

    [Fact]
    public void DeveRecusarAlunoNulo()
    {
        var resultado = AcessoAluno.Criar(1, null!, DateTime.Now);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "ALUNO_OBRIGATORIO");
    }

    [Fact]
    public void DeveRecusarDataHoraPadrao()
    {
        var resultado = AcessoAluno.Criar(1, DomainTestData.Aluno(), default);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "DATA_HORA_OBRIGATORIA");
    }
}