// Nome: Túlio Thauã Dutra
using System.Text.RegularExpressions;
using System.Linq;

namespace AcademiaDoZe.Domain.Services;

public static partial class NormalizadoService
{
    public static bool TextoVazioOuNulo(string? texto) => string.IsNullOrWhiteSpace(texto);

    public static string LimparEspacos(string? texto) =>
        string.IsNullOrWhiteSpace(texto) ? string.Empty : EspacosRegex().Replace(texto, " ").Trim();

    public static string ParaMaiusculo(string? texto) =>
        string.IsNullOrEmpty(texto) ? string.Empty : texto.ToUpperInvariant();

    public static string LimparDigitos(string? texto) =>
        string.IsNullOrEmpty(texto) ? string.Empty : new string(texto.Where(char.IsDigit).ToArray());

    [GeneratedRegex(@"\s+")]
    private static partial Regex EspacosRegex();
}
