using AcademiaDoZe.Domain.Services;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class NormalizadoServiceTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("texto", false)]
    [InlineData("  texto  ", false)]
    public void DeveIdentificarTextoVazio(string? texto, bool esperado)
    {
        Assert.Equal(esperado, NormalizadoService.TextoVazioOuNulo(texto));
    }

    [Theory]
    [InlineData("  texto  ", "texto")]
    [InlineData("texto   maior", "texto maior")]
    [InlineData("  texto   maior  ", "texto maior")]
    [InlineData("a\tb", "a b")]
    [InlineData("uma\nduas", "uma duas")]
    [InlineData(null, "")]
    public void DeveLimparEspacos(string? texto, string esperado)
    {
        Assert.Equal(esperado, NormalizadoService.LimparEspacos(texto));
    }

    [Theory]
    [InlineData("abc123", "123")]
    [InlineData("(11) 98765-4321", "11987654321")]
    [InlineData("sem digitos", "")]
    [InlineData("123", "123")]
    [InlineData(null, "")]
    public void DeveLimparDigitos(string? texto, string esperado)
    {
        Assert.Equal(esperado, NormalizadoService.LimparDigitos(texto));
    }

    [Theory]
    [InlineData("abc", "ABC")]
    [InlineData("Academia do Ze", "ACADEMIA DO ZE")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void DeveConverterParaMaiusculo(string? texto, string esperado)
    {
        Assert.Equal(esperado, NormalizadoService.ParaMaiusculo(texto));
    }
}