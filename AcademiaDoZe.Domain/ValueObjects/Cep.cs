// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cep
{
    public string Valor { get; }

    // Construtor privado impede o uso direto de 'new Cpf()' fora da classe
    private Cep(string valor)
    {
        Valor = valor;
    }

    // Método de Fábrica
    public static Result<Cep> Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return Result<Cep>.Failure("CEP", "CEP_OBRIGATORIO");
        }

        // Normalização: remove pontos e traços para armazenar apenas dígitos
        string cepLimpo = new string(valor.Where(char.IsDigit).ToArray());

        if (cepLimpo.Length != 8)
        {
            return Result<Cep>.Failure("CEP", "CEP_DIGITOS_INVALIDOS");
        }

        return Result<Cep>.Success(new Cep(cepLimpo));
    }

    public override string ToString() => Valor;
}
