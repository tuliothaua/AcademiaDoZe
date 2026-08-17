// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Aluno : Pessoa, IAggregateRoot
{
    private Aluno(
        int id,
        string nome,
        Cpf cpf,
        DateOnly dataNascimento,
        Telefone telefone,
        Email email,
        Senha senha,
        Arquivo foto,
        Logradouro logradouro,
        string numero,
        string complemento)
        : base(id, nome, cpf, dataNascimento, telefone, email, senha, foto, logradouro, numero, complemento)
    {
    }

    public static Result<Aluno> Criar(
        int id,
        string nome,
        string cpfTexto,
        DateOnly dataNascimento,
        string telefoneTexto,
        string emailTexto,
        string senhaTexto,
        Arquivo foto,
        Logradouro logradouro,
        string numero,
        string complemento)
    {
        var notifications = new List<Notification>();

        if (string.IsNullOrWhiteSpace(nome))
            notifications.Add(new Notification("Nome", "NOME_OBRIGATORIO"));
        else
            nome = nome.Trim();

        if (dataNascimento == default)
            notifications.Add(new Notification("DataNascimento", "DATA_NASCIMENTO_OBRIGATORIO"));
        else if (dataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-12)))
            notifications.Add(new Notification("DataNascimento", "DATA_NASCIMENTO_MINIMA_INVALIDA"));

        var cpfResult = Cpf.Criar(cpfTexto);
        if (cpfResult.IsFailure)
            notifications.AddRange(cpfResult.Notifications);

        var telefoneResult = Telefone.Criar(telefoneTexto);
        if (telefoneResult.IsFailure)
            notifications.AddRange(telefoneResult.Notifications);

        var emailResult = Email.Criar(emailTexto);
        if (emailResult.IsFailure)
            notifications.AddRange(emailResult.Notifications);

        var senhaResult = Senha.Criar(senhaTexto);
        if (senhaResult.IsFailure)
            notifications.AddRange(senhaResult.Notifications);

        if (notifications.Count > 0)
            return Result<Aluno>.Failure(notifications);

        var aluno = new Aluno(
            id,
            nome,
            cpfResult.Value!,
            dataNascimento,
            telefoneResult.Value!,
            emailResult.Value!,
            senhaResult.Value!,
            foto,
            logradouro,
            numero,
            complemento);

        return Result<Aluno>.Success(aluno);
    }
}