// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class TelefoneTests
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("11987654321")]
    [InlineData("(11) 98765-4321")]
    [InlineData("55 11 99999999")]
    [InlineData("123456789")]
    [InlineData("abcdefgh12345678")]
    public void DeveCriarTelefoneComOitoOuMaisDigitos(string valor)
    {
        var resultado = Telefone.Criar(valor);
        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeveRecusarTelefoneVazio(string? valor)
    {
        var resultado = Telefone.Criar(valor!);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "TELEFONE_OBRIGATORIO");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData("1234")]
    [InlineData("1234567")]
    public void DeveRecusarTelefoneComPoucosDigitos(string valor)
    {
        var resultado = Telefone.Criar(valor);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "TELEFONE_INVALIDO");
    }

    [Fact]
    public void DeveGuardarSomenteOsDigitos()
    {
        var resultado = Telefone.Criar("(11) 98765-4321");
        Assert.Equal("11987654321", resultado.Value!.Numero);
    }

    [Fact]
    public void DeveRetornarNumeroNoToString()
    {
        var resultado = Telefone.Criar("11987654321");
        Assert.Equal("11987654321", resultado.Value!.ToString());
    }
}