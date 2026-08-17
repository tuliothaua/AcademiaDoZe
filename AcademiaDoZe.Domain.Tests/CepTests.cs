using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class CepTests
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("12.345-678")]
    [InlineData("12345-678")]
    [InlineData("abcdefgh12345678")]
    [InlineData("00000000")]
    public void DeveCriarCepComOitoDigitos(string valor)
    {
        var resultado = Cep.Criar(valor);
        Assert.True(resultado.IsSuccess);
        Assert.Equal(8, resultado.Value!.Valor.Length);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeveRecusarCepVazio(string? valor)
    {
        var resultado = Cep.Criar(valor!);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "CEP_OBRIGATORIO");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("abc")]
    [InlineData("12-34")]
    public void DeveRecusarCepComQuantidadeIncorreta(string valor)
    {
        var resultado = Cep.Criar(valor);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "CEP_DIGITOS_INVALIDOS");
    }

    [Fact]
    public void DeveRetornarCepNoToString()
    {
        var resultado = Cep.Criar("12.345-678");
        Assert.Equal("12345678", resultado.Value!.ToString());
    }
}