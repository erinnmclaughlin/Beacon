using System.Security.Cryptography;
using System.Text;

namespace Beacon.API.Security;

// https://code-maze.com/csharp-hashing-salting-passwords-best-practices/
internal sealed class PasswordHasher : IPasswordHasher
{
    const int _keySize = 64;
    const int _iterations = 350000;
    readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;

    public string Hash(string plainText, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(_keySize);
        var hash = GenerateHash(Encoding.UTF8.GetBytes(plainText), salt);
        return Convert.ToHexString(hash);
    }

    public bool Verify(string plainText, string hash, byte[] salt)
    {
        //var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(plainText, salt, _iterations, _hashAlgorithm, _keySize);
        var hashToCompare = GenerateHash(Encoding.UTF8.GetBytes(plainText), salt);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }

    private byte[] GenerateHash(byte[] bytes, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(bytes, salt, _iterations, _hashAlgorithm, _keySize);
    }
}
