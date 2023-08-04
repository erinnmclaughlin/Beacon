using System.Security.Cryptography;
using System.Text;

namespace DataImporter;

public sealed class PasswordHasher
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
        var hashToCompare = GenerateHash(Encoding.UTF8.GetBytes(plainText), salt);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }

    private byte[] GenerateHash(byte[] bytes, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(bytes, salt, _iterations, _hashAlgorithm, _keySize);
    }
}

