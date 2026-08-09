// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.ValueObjects;
using System.Linq;

namespace AcademiaDoZe.Domain.Entities;

public class Aluno : Pessoa
{
    // Construtor privado
    private Aluno(int id, string nome, Cpf cpf, DateOnly dataNascimento, Telefone telefone, Email email, Senha senha, Arquivo foto, Logradouro logradouro, string numero, string complemento)
        : base(id, nome, cpf, dataNascimento, telefone, email, senha, foto, logradouro, numero, complemento)
    {
    }

    // Método de Fábrica para Aluno
    public static Result<Aluno> Criar(int id, string nome, string cpfStr, DateOnly dataNascimento, string telefoneStr, string emailStr, string senhaStr, Arquivo foto, Logradouro logradouro, string numero, string complemento)
    {
        var notifications = new List<Notification>();

        if (string.IsNullOrWhiteSpace(nome))
        {
            notifications.Add(new Notification("Nome", "NOME_OBRIGATORIO"));
        }

        // Validação e criação do Value Object CPF
        var cpfResult = Cpf.Criar(cpfStr);
        if (cpfResult.IsFailure)
        {
            notifications.AddRange(cpfResult.Notifications);
        }

        // Validação e criação do Telefone
        var telResult = Telefone.Criar(telefoneStr);
        if (telResult.IsFailure)
        {
            notifications.AddRange(telResult.Notifications);
        }

        // Validação e criação do Email
        var emailResult = Email.Criar(emailStr);
        if (emailResult.IsFailure)
        {
            notifications.AddRange(emailResult.Notifications);
        }

        // Validação e criação da Senha
        var senhaResult = Senha.Criar(senhaStr);
        if (senhaResult.IsFailure)
        {
            notifications.AddRange(senhaResult.Notifications);
        }

        if (notifications.Count > 0)
        {
            return Result<Aluno>.Failure(notifications);
        }

        var aluno = new Aluno(
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
            complemento
        );

        return Result<Aluno>.Success(aluno);
    }
}
