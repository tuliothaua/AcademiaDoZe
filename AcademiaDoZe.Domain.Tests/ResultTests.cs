// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;
using System.Linq;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class ResultTests
{
    [Fact]
    public void SuccessDeveMarcarSucesso()
    {
        var resultado = Result<int>.Success(10);
        Assert.True(resultado.IsSuccess);
        Assert.False(resultado.IsFailure);
        Assert.Equal(10, resultado.Value);
    }

    [Fact]
    public void SuccessDeveNaoPossuirNotificacoes()
    {
        var resultado = Result<int>.Success(10);
        Assert.Empty(resultado.Notifications);
    }

    [Fact]
    public void FailureComUmaNotificacaoDeveMarcarFalha()
    {
        var resultado = Result<int>.Failure("Campo", "ERRO");
        Assert.True(resultado.IsFailure);
        Assert.False(resultado.IsSuccess);
    }

    [Fact]
    public void FailureDeveGuardarPropriedade()
    {
        var resultado = Result<int>.Failure("Campo", "ERRO");
        Assert.Equal("Campo", resultado.Notifications.Single().Propriedade);
    }

    [Fact]
    public void FailureDeveGuardarMensagem()
    {
        var resultado = Result<int>.Failure("Campo", "ERRO");
        Assert.Equal("ERRO", resultado.Notifications.Single().Mensagem);
    }

    [Fact]
    public void FailureComListaDeveGuardarTodasAsNotificacoes()
    {
        var lista = new[] { new Notification("A", "1"), new Notification("B", "2") };
        var resultado = Result<int>.Failure(lista);
        Assert.Equal(2, resultado.Notifications.Count);
    }
}