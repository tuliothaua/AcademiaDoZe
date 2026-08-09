// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Endereco
{
    public string Valor { get; }

    private Endereco(string valor)
    {
        Valor = valor;
    }

    public static Result<Endereco> Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return Result<Endereco>.Failure("Endereco", "ENDERECO_OBRIGATORIO");
        }

        return Result<Endereco>.Success(new Endereco(valor.Trim()));
    }

    public override string ToString() => Valor;
}
