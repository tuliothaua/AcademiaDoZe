using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace AcademiaDoZe.Application.Security;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 3;
    private const int MemorySizeKb = 64 * 1024;

    public static string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var argon = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt, DegreeOfParallelism = Math.Max(1, Environment.ProcessorCount),
            MemorySize = MemorySizeKb, Iterations = Iterations
        };
        var hash = argon.GetBytes(HashSize);
        return $"ARGON2ID:{Iterations}:{MemorySizeKb}:{argon.DegreeOfParallelism}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string encoded)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(encoded)) return false;
        var parts = encoded.Split(':');
        if (parts.Length != 6 || parts[0] != "ARGON2ID" || !int.TryParse(parts[1], out var t) ||
            !int.TryParse(parts[2], out var m) || !int.TryParse(parts[3], out var p)) return false;
        try
        {
            var salt = Convert.FromBase64String(parts[4]);
            var expected = Convert.FromBase64String(parts[5]);
            var argon = new Argon2id(Encoding.UTF8.GetBytes(password)) { Salt = salt, Iterations = t, MemorySize = m, DegreeOfParallelism = Math.Max(1, p) };
            return CryptographicOperations.FixedTimeEquals(argon.GetBytes(expected.Length), expected);
        }
        catch { return false; }
    }
}
