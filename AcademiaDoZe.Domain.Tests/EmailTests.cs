using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class EmailTests
{
    [Theory]
    [InlineData("teste@academia.com")]
    [InlineData("aluno@exemplo.com.br")]
    [InlineData(" pessoa@dominio.com ")]
    [InlineData("A@B.C")]
    [InlineData("contato+teste@academia.org")]
    [InlineData("nome.sobrenome@dominio.com")]
    public void DeveCriarEmailValido(string valor)
    {
        var resultado = Email.Criar(valor);
        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeveRecusarEmailVazio(string? valor)
    {
        var resultado = Email.Criar(valor!);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "EMAIL_OBRIGATORIO");
    }

    [Theory]
    [InlineData("email-sem-arroba.com")]
    [InlineData("email@")]
    [InlineData("@dominio.com")]
    [InlineData("email@dominio")]
    [InlineData("email dominio.com")]
    [InlineData("email@dominio.")]
    public void DeveRecusarEmailInvalido(string valor)
    {
        var resultado = Email.Criar(valor);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "EMAIL_INVALIDO");
    }

    [Fact]
    public void DeveNormalizarEmailParaMinusculo()
    {
        var resultado = Email.Criar("  ALUNO@ACADEMIA.COM  ");
        Assert.True(resultado.IsSuccess);
        Assert.Equal("aluno@academia.com", resultado.Value!.Endereco);
    }

    [Fact]
    public void DeveRetornarEmailNoToString()
    {
        var resultado = Email.Criar("aluno@academia.com");
        Assert.Equal("aluno@academia.com", resultado.Value!.ToString());
    }
}