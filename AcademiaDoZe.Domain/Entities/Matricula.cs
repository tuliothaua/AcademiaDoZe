// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Matricula : Entity
{
    public Aluno Aluno { get; private set; }
    public MatriculaPlano Plano { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime DataFim { get; private set; }
    public string Objetivo { get; private set; }
    public MatriculaRestricoes Restricoes { get; private set; }
    public string Observacoes { get; private set; }
    public Arquivo LaudoMedico { get; private set; }

    public Matricula(int id, Aluno aluno, MatriculaPlano plano, DateTime dataInicio, DateTime dataFim, string objetivo, MatriculaRestricoes restricoes, string observacoes, Arquivo laudoMedico)
        : base(id)
    {
        Aluno = aluno;
        Plano = plano;
        DataInicio = dataInicio;
        DataFim = dataFim;
        Objetivo = objetivo;
        Restricoes = restricoes;
        Observacoes = observacoes;
        LaudoMedico = laudoMedico;
    }
}