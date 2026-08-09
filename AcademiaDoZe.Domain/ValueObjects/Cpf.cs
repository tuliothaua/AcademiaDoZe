// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;
using System.Linq;
namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; }

    // Construtor privado impede o uso direto de 'new Cpf()' fora da classe
    private Cpf(string valor)
    {
        Valor = valor;
    }

    // Método de Fábrica
    public static Result<Cpf> Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return Result<Cpf>.Failure("Cpf", "CPF_OBRIGATORIO");
        }

        // Normalização: remove pontos e traços para armazenar apenas dígitos
        string cpfLimpo = new string(valor.Where(char.IsDigit).ToArray());

        if (cpfLimpo.Length != 11)
        {
            return Result<Cpf>.Failure("Cpf", "CPF_DIGITOS_INVALIDOS");
        }

        return Result<Cpf>.Success(new Cpf(cpfLimpo));
    }

    public override string ToString() => Valor;
}
