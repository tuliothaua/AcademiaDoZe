// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class ArquivoTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void DeveCriarArquivoComConteudo(int tamanho)
    {
        var conteudo = new byte[tamanho];
        var resultado = Arquivo.Criar(conteudo);
        Assert.True(resultado.IsSuccess);
        Assert.Equal(tamanho, resultado.Value!.Conteudo.Length);
    }

    [Fact]
    public void DeveRecusarArquivoNulo()
    {
        var resultado = Arquivo.Criar(null!);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "ARQUIVO_VAZIO");
    }

    [Fact]
    public void DeveRecusarArquivoVazio()
    {
        var resultado = Arquivo.Criar([]);
        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "ARQUIVO_VAZIO");
    }

    [Fact]
    public void DeveCopiarOConteudoRecebido()
    {
        var original = new byte[] { 1, 2, 3 };
        var resultado = Arquivo.Criar(original);
        original[0] = 99;
        Assert.Equal(1, resultado.Value!.Conteudo[0]);
    }

    [Fact]
    public void DeveInformarTamanhoNoToString()
    {
        var resultado = Arquivo.Criar(new byte[] { 1, 2, 3 });
        Assert.Equal("Arquivo(3 bytes)", resultado.Value!.ToString());
    }
}