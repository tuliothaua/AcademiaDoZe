// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Domain.Enums;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities;

public class Colaborador : Pessoa
{
    public DateOnly DataAdmissao { get; private set; }
    public ColaboradorTipo Tipo { get; private set; }
    public ColaboradorVinculo Vinculo { get; private set; }

    private Colaborador(int id, string nome, Cpf cpf, DateOnly dataNascimento, Telefone telefone, Email email, Senha senha, Arquivo foto, Logradouro logradouro, string numero, string complemento, DateOnly dataAdmissao, ColaboradorTipo tipo, ColaboradorVinculo vinculo)
        : base(id, nome, cpf, dataNascimento, telefone, email, senha, foto, logradouro, numero, complemento)
    {
        DataAdmissao = dataAdmissao;
        Tipo = tipo;
        Vinculo = vinculo;
    }

    public static Result<Colaborador> Criar(int id, string nome, string cpfStr, DateOnly dataNascimento, string telefoneStr, string emailStr, string senhaStr, Arquivo foto, Logradouro logradouro, string numero, string complemento, DateOnly dataAdmissao, ColaboradorTipo tipo, ColaboradorVinculo vinculo)
    {
        var notifications = new List<Notification>();
        if (tipo == ColaboradorTipo.Administrador && vinculo != ColaboradorVinculo.CLT)
            notifications.Add(new Notification("Vinculo", "ADMINISTRADOR_DEVE_SER_CLT"));

        if (string.IsNullOrWhiteSpace(nome))
            notifications.Add(new Notification("Nome", "NOME_OBRIGATORIO"));

        var cpfResult = Cpf.Criar(cpfStr);
        if (cpfResult.IsFailure)
            notifications.AddRange(cpfResult.Notifications);

        var telResult = Telefone.Criar(telefoneStr);
        if (telResult.IsFailure)
            notifications.AddRange(telResult.Notifications);

        var emailResult = Email.Criar(emailStr);
        if (emailResult.IsFailure)
            notifications.AddRange(emailResult.Notifications);

        var senhaResult = Senha.Criar(senhaStr);
        if (senhaResult.IsFailure)
            notifications.AddRange(senhaResult.Notifications);

        if (notifications.Count > 0)
            return Result<Colaborador>.Failure(notifications);

        var colaborador = new Colaborador(
            id,
            nome.Trim(),
            cpfResult.Value!,
            dataNascimento,
            telResult.Value!,
            emailResult.Value!,
            senhaResult.Value!,
            foto,
            logradouro,
            numero,
            complemento,
            dataAdmissao,
            tipo,
            vinculo
        );

        return Result<Colaborador>.Success(colaborador);
    }
}