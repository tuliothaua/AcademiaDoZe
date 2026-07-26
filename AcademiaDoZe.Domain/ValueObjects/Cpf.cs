// Nome: Túlio Thauã Dutra
namespace AcademiaDoZe.Domain.ValueObjects;

// Records facilitam a criação de objetos imutáveis e comparação por valor [5]
public record Cpf
{
    public string Valor { get; }

    public Cpf(string valor)
    {
        // Aqui futuramente você pode adicionar a lógica de validação do CPF [7]
        Valor = valor;
    }
}