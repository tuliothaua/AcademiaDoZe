// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Arquivo
{
    public byte[] Conteudo { get; }

    private Arquivo(byte[] conteudo)
    {
        Conteudo = conteudo;
    }

    public static Result<Arquivo> Criar(byte[] conteudo)
    {
        if (conteudo == null || conteudo.Length == 0)
        {
            return Result<Arquivo>.Failure("Arquivo", "ARQUIVO_VAZIO");
        }

        return Result<Arquivo>.Success(new Arquivo((byte[])conteudo.Clone()));
    }

    public override string ToString() => $"Arquivo({Conteudo?.Length ?? 0} bytes)";
}
