using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests;

public class CpfTests
{
    [Fact]
    public void DeveCriarCpfComOnzeDigitos()
    {
        var resultado = Cpf.Criar("12345678901");

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
    }

    [Fact]
    public void DeveAceitarCpfComPontuacao()
    {
        var resultado = Cpf.Criar("123.456.789-01");

        Assert.True(resultado.IsSuccess);
        Assert.Equal("12345678901", resultado.Value!.Valor);
    }

    [Fact]
    public void DeveRecusarCpfVazio()
    {
        var resultado = Cpf.Criar("");

        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, notificacao =>
            notificacao.Mensagem == "CPF_OBRIGATORIO");
    }

    [Fact]
    public void DeveRecusarCpfComQuantidadeIncorretaDeDigitos()
    {
        var resultado = Cpf.Criar("123456");

        Assert.True(resultado.IsFailure);
        Assert.Contains(resultado.Notifications, notificacao =>
            notificacao.Mensagem == "CPF_DIGITOS_INVALIDOS");
    }

}
