// Nome: Túlio Thauã Dutra
using System.Text.RegularExpressions;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Email
{
    public string Endereco { get; }

    private Email(string endereco)
    {
        Endereco = endereco;
    }

    public static Result<Email> Criar(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
        {
            return Result<Email>.Failure("Email", "EMAIL_OBRIGATORIO");
        }

        var trimmed = endereco.Trim();

        // Simples validação de formato
        if (!Regex.IsMatch(trimmed, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return Result<Email>.Failure("Email", "EMAIL_INVALIDO");
        }

        return Result<Email>.Success(new Email(trimmed.ToLowerInvariant()));
    }

    public override string ToString() => Endereco;
}
