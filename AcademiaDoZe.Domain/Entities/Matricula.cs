// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Entities;

public class Matricula : Entity, IAggregateRoot
{
    public Aluno AlunoMatricula { get; private set; }
    public MatriculaPlano Plano { get; private set; }
    public DateOnly DataInicio { get; private set; }
    public DateOnly? DataFim { get; private set; }
    public string Objetivo { get; private set; }
    public MatriculaRestricoes RestricoesMedicas { get; private set; }
    public string ObservacoesRestricoes { get; private set; }
    public Arquivo? LaudoMedico { get; private set; }

    private Matricula(int id, Aluno aluno, MatriculaPlano plano, DateOnly dataInicio, DateOnly? dataFim, string objetivo, MatriculaRestricoes restricoes, string observacoes, Arquivo? laudo)
        : base(id)
    {
        AlunoMatricula = aluno;
        Plano = plano;
        DataInicio = dataInicio;
        DataFim = dataFim;
        Objetivo = objetivo;
        RestricoesMedicas = restricoes;
        ObservacoesRestricoes = observacoes;
        LaudoMedico = laudo;
    }

    public static Result<Matricula> Criar(int id, Aluno aluno, MatriculaPlano plano, DateOnly dataInicio, DateOnly? dataFim, string objetivo, MatriculaRestricoes restricoes, string observacoes, Arquivo? laudo)
    {
        var notifications = new List<Notification>();

        if (aluno == null)
            notifications.Add(new Notification("Aluno", "ALUNO_OBRIGATORIO"));

        if (string.IsNullOrWhiteSpace(objetivo))
            notifications.Add(new Notification("Objetivo", "OBJETIVO_OBRIGATORIO"));

        if (dataInicio == default)
            notifications.Add(new Notification("DataInicio", "DATA_INICIO_OBRIGATORIA"));

        if (notifications.Count > 0)
            return Result<Matricula>.Failure(notifications);

        return Result<Matricula>.Success(new Matricula(id, aluno!, plano, dataInicio, dataFim, objetivo, restricoes, observacoes, laudo));
    }
}