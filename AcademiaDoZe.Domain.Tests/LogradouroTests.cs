using AcademiaDoZe.Domain.Entities;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class LogradouroTests
{
    private static Logradouro CriarValido()
    {
        return Logradouro.Criar(1, "12345678", "Brasil", "SP", "Sao Paulo", "Centro", "Rua A").Value!;
    }

    [Fact]
    public void DeveCriarLogradouroValido()
    {
        var resultado = Logradouro.Criar(1, "12345678", "Brasil", "SP", "Sao Paulo", "Centro", "Rua A");
        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
    }

    [Fact]
    public void DeveGuardarId()
    {
        Assert.Equal(1, CriarValido().Id);
    }

    [Fact]
    public void DeveGuardarCep()
    {
        Assert.Equal("12345678", CriarValido().Cep.Valor);
    }

    [Fact]
    public void DeveGuardarPais()
    {
        Assert.Equal("Brasil", CriarValido().Pais);
    }

    [Fact]
    public void DeveGuardarEstado()
    {
        Assert.Equal("SP", CriarValido().Estado);
    }

    [Fact]
    public void DeveGuardarCidade()
    {
        Assert.Equal("Sao Paulo", CriarValido().Cidade);
    }

    [Fact]
    public void DeveGuardarBairro()
    {
        Assert.Equal("Centro", CriarValido().Bairro);
    }

    [Fact]
    public void DeveGuardarNome()
    {
        Assert.Equal("Rua A", CriarValido().Nome);
    }

    [Fact]
    public void DeveRecusarCepInvalido()
    {
        var resultado = Logradouro.Criar(1, "123", "Brasil", "SP", "Sao Paulo", "Centro", "Rua A");
        Assert.True(resultado.IsFailure);
    }

    [Fact]
    public void DeveRecusarPaisVazio()
    {
        var resultado = Logradouro.Criar(1, "12345678", "", "SP", "Sao Paulo", "Centro", "Rua A");
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "PAIS_OBRIGATORIO");
    }

    [Fact]
    public void DeveRecusarEstadoVazio()
    {
        var resultado = Logradouro.Criar(1, "12345678", "Brasil", "", "Sao Paulo", "Centro", "Rua A");
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "ESTADO_OBRIGATORIO");
    }

    [Fact]
    public void DeveRecusarCidadeVazia()
    {
        var resultado = Logradouro.Criar(1, "12345678", "Brasil", "SP", "", "Centro", "Rua A");
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "CIDADE_OBRIGATORIA");
    }

    [Fact]
    public void DeveRecusarNomeVazio()
    {
        var resultado = Logradouro.Criar(1, "12345678", "Brasil", "SP", "Sao Paulo", "Centro", "");
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "NOME_LOGRADOURO_OBRIGATORIO");
    }
}