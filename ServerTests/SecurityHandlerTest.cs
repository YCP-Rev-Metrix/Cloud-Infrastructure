﻿using Server.Security;
using TestCommon;

namespace ServerTests;

public class SecurityHandlerTest
{
    /// <summary>
    /// This is stateless and may therefore have only one instance
    /// </summary>
    public SecurityHandler SecurityHandler = new();

    [Trait("Category", "Security Critical Test")]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1)]
    [InlineData(9535821)]
    [Theory]
    public void GenerateRandomBytesLengthTest(int length)
    {
        byte[] bytes = SecurityHandler.GenerateRandomBytes(length);

        Assert.Equal(length, bytes.Length);
    }

    [Trait("Category", "Security Critical Test")]
    [Fact]
    public void GenerateRefreshTokenTest()
    {
        byte[] token = SecurityHandler.GenerateRefreshToken();

        Assert.Equal(32, token.Length);
    }

    [Trait("Category", "Security Critical Test")]
    [MemberData(nameof(PasswordTestData))]
    [Theory]
    public void SaltHashLengthTest(string password)
    {
        (byte[] hash, byte[] salt) = SecurityHandler.SaltHashPassword(password);

        Assert.Equal(32, hash.Length);

        Assert.Equal(16, salt.Length);
    }

    [Trait("Category", "Security Critical Test")]
    [MemberData(nameof(PasswordTestData))]
    [Theory]
    public void PasswordHashEqualTest(string password)
    {
        (byte[] hash, byte[] salt) = SecurityHandler.SaltHashPassword(password);

        byte[] rehashed = SecurityHandler.SaltHashPassword(password, salt);

        AssertExtensions.SequenceEquals(hash, rehashed);
    }

    public static IEnumerable<object[]> PasswordTestData()
    {
        yield return new object[] { "strong🥹! password yes very" };
        yield return new object[] { "hdsfghjkdxfgchjxgfhjfkjjkfdg~hjgf@dghjklhhgjfjhjglkghgdfjdfhgljghgfjfhgjhgfjhjglkhfgdfhgjlhgfgjghgfsg[mpcwofweuimcewru" };
        yield return new object[] { "e$\"C]?;@9A`3=&j2h/x_y7m[nwFWqM8r,>fa'+^GVd" };
        yield return new object[] { "." };
    }
}