using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class ColaboradorTests
{
    [Fact]
    public void DeveCriarColaboradorValido()
    {
        Assert.True(DomainTestData.Colaborador().Id > 0);
    }

    [Fact]
    public void DeveGuardarTipo()
    {
        var resultado = Colaborador.Criar(1, "Nome", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), "11987654321", "nome@academia.com", "senha123", DomainTestData.Foto(), DomainTestData.Logradouro(), "1", "", DateOnly.FromDateTime(DateTime.Today), ColaboradorTipo.Instrutor, ColaboradorVinculo.CLT);
        Assert.Equal(ColaboradorTipo.Instrutor, resultado.Value!.Tipo);
    }

    [Fact]
    public void DeveGuardarVinculo()
    {
        Assert.Equal(ColaboradorVinculo.CLT, DomainTestData.Colaborador().Vinculo);
    }

    [Fact]
    public void DeveRecusarAdministradorSemCLT()
    {
        var resultado = Colaborador.Criar(1, "Nome", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), "11987654321", "nome@academia.com", "senha123", DomainTestData.Foto(), DomainTestData.Logradouro(), "1", "", DateOnly.FromDateTime(DateTime.Today), ColaboradorTipo.Administrador, ColaboradorVinculo.Estagio);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "ADMINISTRADOR_DEVE_SER_CLT");
    }

    [Fact]
    public void DeveAceitarAdministradorCLT()
    {
        var resultado = Colaborador.Criar(1, "Nome", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), "11987654321", "nome@academia.com", "senha123", DomainTestData.Foto(), DomainTestData.Logradouro(), "1", "", DateOnly.FromDateTime(DateTime.Today), ColaboradorTipo.Administrador, ColaboradorVinculo.CLT);
        Assert.True(resultado.IsSuccess);
    }

    [Fact]
    public void DeveRecusarNomeVazio()
    {
        var resultado = Colaborador.Criar(1, "", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), "11987654321", "nome@academia.com", "senha123", DomainTestData.Foto(), DomainTestData.Logradouro(), "1", "", DateOnly.FromDateTime(DateTime.Today), ColaboradorTipo.Atendente, ColaboradorVinculo.CLT);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "NOME_OBRIGATORIO");
    }

    [Theory]
    [InlineData(ColaboradorTipo.Atendente)]
    [InlineData(ColaboradorTipo.Instrutor)]
    [InlineData(ColaboradorTipo.Administrador)]
    public void DeveAceitarTiposComCLT(ColaboradorTipo tipo)
    {
        var resultado = Colaborador.Criar(1, "Nome", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), "11987654321", "nome@academia.com", "senha123", DomainTestData.Foto(), DomainTestData.Logradouro(), "1", "", DateOnly.FromDateTime(DateTime.Today), tipo, ColaboradorVinculo.CLT);
        Assert.True(resultado.IsSuccess);
    }

    [Fact]
    public void DeveRecusarCpfInvalido()
    {
        var resultado = Colaborador.Criar(1, "Nome", "123", DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), "11987654321", "nome@academia.com", "senha123", DomainTestData.Foto(), DomainTestData.Logradouro(), "1", "", DateOnly.FromDateTime(DateTime.Today), ColaboradorTipo.Atendente, ColaboradorVinculo.CLT);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "CPF_DIGITOS_INVALIDOS");
    }

    [Fact]
    public void DeveRecusarEmailInvalido()
    {
        var resultado = Colaborador.Criar(1, "Nome", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), "11987654321", "invalido", "senha123", DomainTestData.Foto(), DomainTestData.Logradouro(), "1", "", DateOnly.FromDateTime(DateTime.Today), ColaboradorTipo.Atendente, ColaboradorVinculo.CLT);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "EMAIL_INVALIDO");
    }

    [Fact]
    public void DeveRecusarSenhaInvalida()
    {
        var resultado = Colaborador.Criar(1, "Nome", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), "11987654321", "nome@academia.com", "123", DomainTestData.Foto(), DomainTestData.Logradouro(), "1", "", DateOnly.FromDateTime(DateTime.Today), ColaboradorTipo.Atendente, ColaboradorVinculo.CLT);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "SENHA_MINIMA_6");
    }
}