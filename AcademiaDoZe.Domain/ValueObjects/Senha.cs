// Nome: Túlio Thauã Dutra
namespace AcademiaDoZe.Domain.ValueObjects;

// Records facilitam a criação de objetos imutáveis e comparação por valor [5]
public record Senha
{
    public string Valor { get; }

    public Senha(string valor)
    {
        Valor = valor;
    }
}