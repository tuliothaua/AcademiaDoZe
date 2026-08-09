// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects;

// Records facilitam a criação de objetos imutáveis e comparação por valor [5]
public record Senha
{
    public string Valor { get; }

    private Senha(string valor)
    {
        Valor = valor;
    }

    public static Result<Senha> Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return Result<Senha>.Failure("Senha", "SENHA_OBRIGATORIA");
        }

        if (valor.Length < 6)
        {
            return Result<Senha>.Failure("Senha", "SENHA_MINIMA_6");
        }

        return Result<Senha>.Success(new Senha(valor));
    }
}
