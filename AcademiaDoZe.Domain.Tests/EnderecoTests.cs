// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class EnderecoTests
{
    [Theory]
    [InlineData("Rua A, 10")]
    [InlineData("Avenida Central")]
    [InlineData("Casa 5")]
    [InlineData("Rua das Flores")]
    [InlineData("Praça Principal")]
    public void DeveCriarEnderecoValido(string valor)
    {
        var resultado = Endereco.Criar(valor);
        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeveRecusarEnderecoVazio(string? valor)
    {
        var resultado = Endereco.Criar(valor!);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "ENDERECO_OBRIGATORIO");
    }

    [Fact]
    public void DeveRemoverEspacosDasExtremidades()
    {
        var resultado = Endereco.Criar("  Rua A  ");
        Assert.Equal("Rua A", resultado.Value!.Valor);
    }

    [Fact]
    public void DeveRetornarValorNoToString()
    {
        var resultado = Endereco.Criar("Rua A");
        Assert.Equal("Rua A", resultado.Value!.ToString());
    }
}