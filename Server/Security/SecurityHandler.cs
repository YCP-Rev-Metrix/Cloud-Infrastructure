using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Server.Security;

public class SecurityHandler
{
    private readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();

    public readonly SymmetricSecurityKey AuthorizationSigningTokenKey;
    public readonly SigningCredentials AuthorizationSigningCredentials;

    public SecurityHandler()
    {
        AuthorizationSigningTokenKey = GenerateSymmetricKey(Config.AuthSecretLength);
        AuthorizationSigningCredentials = new(AuthorizationSigningTokenKey, SecurityAlgorithms.HmacSha256Signature);
    }

    public byte[] GenerateRandomBytes(int length)
    {
        byte[] randomBytes = new byte[length];
        RandomNumberGenerator.GetBytes(randomBytes);
        return randomBytes;
    }

    public byte[] GenerateRefreshToken() => GenerateRandomBytes(32);

    private SymmetricSecurityKey GenerateSymmetricKey(int length) => new(GenerateRandomBytes(length));

    public (byte[] hashed, byte[] salt) SaltHashPassword(string password)
    {
        byte[] salt = GenerateRandomBytes(16);
        byte[] hashed = SaltHashPassword(password, salt);
        return (hashed, salt);
    }

    public byte[] SaltHashPassword(string password, byte[] salt)
    {
        byte[] passwordbytes = Encoding.ASCII.GetBytes(password);
        var s = new MemoryStream();
        s.Write(passwordbytes, 0, passwordbytes.Length);
        s.Write(salt, 0, salt.Length);
        byte[] combined = s.ToArray();
        byte[] hashed = SHA256.HashData(combined);
        return hashed;
    }
}
