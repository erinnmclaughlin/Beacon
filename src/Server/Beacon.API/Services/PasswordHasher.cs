using System.Security.Cryptography;
using System.Text;

namespace Beacon.API.Services;

public interface IPasswordHasher
{
    string Hash(string plainText, out byte[] salt);
    bool Verify(string plainText, string hash, byte[] salt);
}

// https://code-maze.com/csharp-hashing-salting-passwords-best-practices/
public sealed class PasswordHasher : IPasswordHasher
{
    private const int KeySize = 64;
    private const int Iterations = 350000;
    private readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;

    public string Hash(string plainText, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(KeySize);
        var hash = GenerateHash(Encoding.UTF8.GetBytes(plainText), salt);
        return Convert.ToHexString(hash);
    }

    public bool Verify(string plainText, string hash, byte[] salt)
    {
        var hashToCompare = GenerateHash(Encoding.UTF8.GetBytes(plainText), salt);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }

    private byte[] GenerateHash(byte[] bytes, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(bytes, salt, Iterations, _hashAlgorithm, KeySize);
    }
}
