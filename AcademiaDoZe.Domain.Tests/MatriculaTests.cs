// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using Xunit;

namespace AcademiaDoZe.Domain.Tests;

public class MatriculaTests
{
    private static Matricula CriarValida()
    {
        return Matricula.Criar(1, DomainTestData.Aluno(), MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), null, "Ganhar massa", MatriculaRestricoes.None, "", null).Value!;
    }

    [Fact]
    public void DeveCriarMatriculaValida()
    {
        Assert.NotNull(CriarValida());
    }

    [Fact]
    public void DeveGuardarAluno()
    {
        Assert.Equal(DomainTestData.Aluno().Id, CriarValida().AlunoMatricula.Id);
    }

    [Fact]
    public void DeveGuardarPlano()
    {
        Assert.Equal(MatriculaPlano.Mensal, CriarValida().Plano);
    }

    [Fact]
    public void DeveGuardarDataInicio()
    {
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), CriarValida().DataInicio);
    }

    [Fact]
    public void DeveGuardarObjetivo()
    {
        Assert.Equal("Ganhar massa", CriarValida().Objetivo);
    }

    [Fact]
    public void DeveRecusarAlunoNulo()
    {
        var resultado = Matricula.Criar(1, null!, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), null, "Objetivo", MatriculaRestricoes.None, "", null);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "ALUNO_OBRIGATORIO");
    }

    [Fact]
    public void DeveRecusarObjetivoVazio()
    {
        var resultado = Matricula.Criar(1, DomainTestData.Aluno(), MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), null, "", MatriculaRestricoes.None, "", null);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "OBJETIVO_OBRIGATORIO");
    }

    [Fact]
    public void DeveRecusarDataInicioPadrao()
    {
        var resultado = Matricula.Criar(1, DomainTestData.Aluno(), MatriculaPlano.Mensal, default, null, "Objetivo", MatriculaRestricoes.None, "", null);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "DATA_INICIO_OBRIGATORIA");
    }

    [Fact]
    public void DeveExigirLaudoParaRestricaoMedica()
    {
        var resultado = Matricula.Criar(1, DomainTestData.Aluno(), MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), null, "Objetivo", MatriculaRestricoes.Diabetes, "", null);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "LAUDO_MEDICO_OBRIGATORIO_POR_RESTRICAO");
    }

    [Fact]
    public void DeveExigirLaudoParaAlunoDeDezesseisAnos()
    {
        var nascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-16));
        var aluno = Aluno.Criar(8, "Aluno Jovem", "12345678901", nascimento, "11987654321", "jovem@academia.com", "senha123", DomainTestData.Foto(), DomainTestData.Logradouro(), "10", "").Value!;
        var resultado = Matricula.Criar(1, aluno, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), null, "Objetivo", MatriculaRestricoes.None, "", null);
        Assert.Contains(resultado.Notifications, n => n.Mensagem == "LAUDO_MEDICO_OBRIGATORIO_POR_IDADE");
    }
}

//Aluno: Túlio Thauã Dutra
