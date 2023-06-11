namespace Beacon.App.Services;

public interface IPasswordHasher
{
    string Hash(string plainText, out byte[] salt);
    bool Verify(string plainText, string hash, byte[] salt);
}
