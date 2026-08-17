using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class AlunoTests
{
    private static object[] DadosValidos()
    {
        return new object[]
        {
            1, "Aluno", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
            "11987654321", "aluno@academia.com", "senha123", DomainTestData.Foto(),
            DomainTestData.Logradouro(), "10", ""
        };
    }

    [Fact]
    public void DeveCriarAlunoValido()
    {
        var d = DadosValidos();
        var resultado = Aluno.Criar((int)d[0], (string)d[1], (string)d[2], (DateOnly)d[3], (string)d[4], (string)d[5], (string)d[6], (Arquivo)d[7], (Logradouro)d[8], (string)d[9], (string)d[10]);
        Assert.True(resultado.IsSuccess);
    }

    [Fact]
    public void DeveGuardarNome()
    {
        Assert.Equal("Aluno Teste", DomainTestData.Aluno().NomeCompleto);
    }

    [Fact]
    public void DeveRemoverEspacosDoNome()
    {
        var d = DadosValidos();
        d[1] = "  Aluno  ";
        var resultado = Aluno.Criar((int)d[0], (string)d[1], (string)d[2], (DateOnly)d[3], (string)d[4], (string)d[5], (string)d[6], (Arquivo)d[7], (Logradouro)d[8], (string)d[9], (string)d[10]);
        Assert.Equal("Aluno", resultado.Value!.NomeCompleto);
    }

    [Fact]
    public void DeveRecusarNomeVazio()
    {
        var d = DadosValidos(); d[1] = "";
        var resultado = Aluno.Criar((int)d[0], (string)d[1], (string)d[2], (DateOnly)d[3], (string)d[4], (string)d[5], (string)d[6], (Arquivo)d[7], (Logradouro)d[8], (string)d[9], (string)d[10]);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "NOME_OBRIGATORIO");
    }

    [Fact]
    public void DeveRecusarDataNascimentoPadrao()
    {
        var d = DadosValidos(); d[3] = default(DateOnly);
        var resultado = Aluno.Criar((int)d[0], (string)d[1], (string)d[2], (DateOnly)d[3], (string)d[4], (string)d[5], (string)d[6], (Arquivo)d[7], (Logradouro)d[8], (string)d[9], (string)d[10]);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "DATA_NASCIMENTO_OBRIGATORIO");
    }

    [Fact]
    public void DeveRecusarAlunoComMenosDeDozeAnos()
    {
        var d = DadosValidos(); d[3] = DateOnly.FromDateTime(DateTime.Today.AddYears(-11));
        var resultado = Aluno.Criar((int)d[0], (string)d[1], (string)d[2], (DateOnly)d[3], (string)d[4], (string)d[5], (string)d[6], (Arquivo)d[7], (Logradouro)d[8], (string)d[9], (string)d[10]);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "DATA_NASCIMENTO_MINIMA_INVALIDA");
    }

    [Fact]
    public void DeveRecusarCpfInvalido()
    {
        var d = DadosValidos(); d[2] = "123";
        var resultado = Aluno.Criar((int)d[0], (string)d[1], (string)d[2], (DateOnly)d[3], (string)d[4], (string)d[5], (string)d[6], (Arquivo)d[7], (Logradouro)d[8], (string)d[9], (string)d[10]);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "CPF_DIGITOS_INVALIDOS");
    }

    [Fact]
    public void DeveRecusarTelefoneInvalido()
    {
        var d = DadosValidos(); d[4] = "123";
        var resultado = Aluno.Criar((int)d[0], (string)d[1], (string)d[2], (DateOnly)d[3], (string)d[4], (string)d[5], (string)d[6], (Arquivo)d[7], (Logradouro)d[8], (string)d[9], (string)d[10]);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "TELEFONE_INVALIDO");
    }

    [Fact]
    public void DeveRecusarEmailInvalido()
    {
        var d = DadosValidos(); d[5] = "email-invalido";
        var resultado = Aluno.Criar((int)d[0], (string)d[1], (string)d[2], (DateOnly)d[3], (string)d[4], (string)d[5], (string)d[6], (Arquivo)d[7], (Logradouro)d[8], (string)d[9], (string)d[10]);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "EMAIL_INVALIDO");
    }

    [Fact]
    public void DeveRecusarSenhaInvalida()
    {
        var d = DadosValidos(); d[6] = "123";
        var resultado = Aluno.Criar((int)d[0], (string)d[1], (string)d[2], (DateOnly)d[3], (string)d[4], (string)d[5], (string)d[6], (Arquivo)d[7], (Logradouro)d[8], (string)d[9], (string)d[10]);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "SENHA_MINIMA_6");
    }

    [Fact]
    public void DeveAcumularMaisDeUmaNotificacao()
    {
        var d = DadosValidos(); d[1] = ""; d[2] = "123"; d[4] = "123";
        var resultado = Aluno.Criar((int)d[0], (string)d[1], (string)d[2], (DateOnly)d[3], (string)d[4], (string)d[5], (string)d[6], (Arquivo)d[7], (Logradouro)d[8], (string)d[9], (string)d[10]);
        Assert.True(resultado.Notifications.Count >= 3);
    }
}