using AcademiaDoZe.Domain.Entities;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class AcessoColaboradorTests
{
    [Fact]
    public void DeveCriarAcessoValido()
    {
        var resultado = AcessoColaborador.Criar(1, DomainTestData.Colaborador(), DateTime.Now);
        Assert.True(resultado.IsSuccess);
    }

    [Fact]
    public void DeveGuardarColaborador()
    {
        var resultado = AcessoColaborador.Criar(1, DomainTestData.Colaborador(), DateTime.Now);
        Assert.Equal(DomainTestData.Colaborador().Id, resultado.Value!.Colaborador.Id);
    }

    [Fact]
    public void DeveRecusarColaboradorNulo()
    {
        var resultado = AcessoColaborador.Criar(1, null!, DateTime.Now);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "COLABORADOR_OBRIGATORIO");
    }

    [Fact]
    public void DeveRecusarDataHoraPadrao()
    {
        var resultado = AcessoColaborador.Criar(1, DomainTestData.Colaborador(), default);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "DATA_HORA_OBRIGATORIA");
    }
}