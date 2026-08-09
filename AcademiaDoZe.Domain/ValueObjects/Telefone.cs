// Nome: Túlio Thauã Dutra
using System.Linq;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Telefone
{
    public string Numero { get; }

    private Telefone(string numero)
    {
        Numero = numero;
    }

    public static Result<Telefone> Criar(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
        {
            return Result<Telefone>.Failure("Telefone", "TELEFONE_OBRIGATORIO");
        }

        var somenteDigitos = new string(numero.Where(char.IsDigit).ToArray());

        if (somenteDigitos.Length < 8)
        {
            return Result<Telefone>.Failure("Telefone", "TELEFONE_INVALIDO");
        }

        return Result<Telefone>.Success(new Telefone(somenteDigitos));
    }

    public override string ToString() => Numero;
}
