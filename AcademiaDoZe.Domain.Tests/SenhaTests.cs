using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class SenhaTests
{
    [Theory]
    [InlineData("abc123")]
    [InlineData("senha123")]
    [InlineData("123456")]
    [InlineData("SenhaForte")]
    [InlineData("abcdefghi")]
    public void DeveCriarSenhaComSeisOuMaisCaracteres(string valor)
    {
        var resultado = Senha.Criar(valor);
        Assert.True(resultado.IsSuccess);
        Assert.Equal(valor, resultado.Value!.Valor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeveRecusarSenhaVazia(string? valor)
    {
        var resultado = Senha.Criar(valor!);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "SENHA_OBRIGATORIA");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData("1234")]
    [InlineData("12345")]
    public void DeveRecusarSenhaComMenosDeSeisCaracteres(string valor)
    {
        var resultado = Senha.Criar(valor);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "SENHA_MINIMA_6");
    }

    [Fact]
    public void DeveManterASenhaOriginal() { }
}