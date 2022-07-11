using System;
using System.Security.Cryptography;
using Clinia.Cryptography.Pbkdf2;
using Xunit;

namespace Clinia.Cryptography.Test;

public class Pbkdf2HasherTest
{
    [Fact]
    public void Ctor_InvalidCompatMode_Throws()
    {
        // Act & assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            new Pbkdf2HasherImpl(compatMode: (Pbkdf2HasherCompatibilityMode)(-1));
        });
        Assert.Equal("The provided HasherCompatibilityMode is invalid.", ex.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Ctor_InvalidIterCount_Throws(int iterCount)
    {
        // Act & assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            new Pbkdf2HasherImpl(iterCount: iterCount);
        });
        Assert.Equal("The iteration count must be a positive integer.", ex.Message);
    }

    [Theory]
    [InlineData(Pbkdf2HasherCompatibilityMode.V1)]
    public void FullRoundTrip(Pbkdf2HasherCompatibilityMode compatMode)
    {
        // Arrange
        var hasher = new Pbkdf2HasherImpl(compatMode: compatMode);

        // Act & assert - success case
        var hashedString = hasher.Generate("string 1");
        var successResult = hasher.Compare(hashedString, "string 1");
        Assert.Equal(HasherVerificationResult.Success, successResult);

        // Act & assert - failure case
        var failedResult = hasher.Compare(hashedString, "string 2");
        Assert.Equal(HasherVerificationResult.Failed, failedResult);
    }

    [Fact]
    public void HashString_DefaultsToVersion1()
    {
        // Arrange
        var hasher = new Pbkdf2HasherImpl(compatMode: null);

        // Act
        string retVal = hasher.Generate("my password");

        // Assert
        Assert.Equal("AQAAAAEAACcQAAAAEAABAgMEBQYHCAkKCwwNDg+yWU7rLgUwPZb1Itsmra7cbxw2EFpwpVFIEtP+JIuUEw==", retVal);
    }

    [Fact]
    public void HashString_Version1()
    {
        // Arrange
        var hasher = new Pbkdf2HasherImpl(compatMode: Pbkdf2HasherCompatibilityMode.V1);

        // Act
        var retVal = hasher.Generate("my password");

        // Assert
        Assert.Equal("AQAAAAEAACcQAAAAEAABAgMEBQYHCAkKCwwNDg+yWU7rLgUwPZb1Itsmra7cbxw2EFpwpVFIEtP+JIuUEw==", retVal);
    }

    [Theory]
    // Version 1 payloads
    [InlineData("AQAAAAAAAAD6AAAAEAhftMyfTJyAAAAAAAAAAAAAAAAAAAih5WsjXaR3PA9M")] // incorrect string
    [InlineData("AQAAAAIAAAAyAAAAEOMwvh3+FZxqkdMBz2ekgGhwQ4A=")] // too short
    [InlineData("AQAAAAIAAAAyAAAAEOMwvh3+FZxqkdMBz2ekgGhwQ4B6pZWND6zgESBuWiHwAAAAAAAAAAAA")] // extra data at end
    public void VerifyHashedString_FailureCases(string hashedString)
    {
        // Arrange
        var hasher = new Pbkdf2Hasher();

        // Act
        var result = hasher.Compare(hashedString, "my string");

        // Assert
        Assert.Equal(HasherVerificationResult.Failed, result);
    }

    [Theory]
    // Version 1 payloads
    [InlineData("AQAAAAIAAAAyAAAAEOMwvh3+FZxqkdMBz2ekgGhwQ4B6pZWND6zgESBuWiHw", HasherVerificationResult.SuccessRehashNeeded)] // SHA512, 50 iterations, 128-bit salt, 128-bit subkey
    [InlineData("AQAAAAIAAAD6AAAAIJbVi5wbMR+htSfFp8fTw8N8GOS/Sje+S/4YZcgBfU7EQuqv4OkVYmc4VJl9AGZzmRTxSkP7LtVi9IWyUxX8IAAfZ8v+ZfhjCcudtC1YERSqE1OEdXLW9VukPuJWBBjLuw==", HasherVerificationResult.SuccessRehashNeeded)] // SHA512, 250 iterations, 256-bit salt, 512-bit subkey
    [InlineData("AQAAAAAAAAD6AAAAEAhftMyfTJylOlZT+eEotFXd1elee8ih5WsjXaR3PA9M", HasherVerificationResult.SuccessRehashNeeded)] // SHA1, 250 iterations, 128-bit salt, 128-bit subkey
    [InlineData("AQAAAAEAA9CQAAAAIESkQuj2Du8Y+kbc5lcN/W/3NiAZFEm11P27nrSN5/tId+bR1SwV8CO1Jd72r4C08OLvplNlCDc3oQZ8efcW+jQ=", HasherVerificationResult.Success)] // SHA256, 250000 iterations, 256-bit salt, 256-bit subkey
    public void VerifyHashedString_Version1CompatMode_SuccessCases(string hashedString, HasherVerificationResult expectedResult)
    {
        // Arrange
        var hasher = new Pbkdf2HasherImpl(compatMode: Pbkdf2HasherCompatibilityMode.V1);

        // Act
        var actualResult = hasher.Compare(hashedString, "my password");

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }

    private sealed class Pbkdf2HasherImpl : Pbkdf2Hasher
    {
        public Pbkdf2HasherImpl(Pbkdf2HasherCompatibilityMode? compatMode = null, int? iterCount = null)
            : base(BuildOptions(compatMode, iterCount))
        {
        }

        private static Pbkdf2HasherOptions BuildOptions(Pbkdf2HasherCompatibilityMode? compatMode, int? iterCount)
        {
            var options = new StringHasherOptionsAccessor();
            if (compatMode != null)
            {
                options.Value.CompatibilityMode = (Pbkdf2HasherCompatibilityMode)compatMode;
            }
            if (iterCount != null)
            {
                options.Value.IterationCount = (int)iterCount;
            }
            Assert.NotNull(options.Value.Rng); // should have a default value
            options.Value.Rng = new SequentialRandomNumberGenerator();
            return options.Value;
        }
    }

    private sealed class SequentialRandomNumberGenerator : RandomNumberGenerator
    {
        private byte _value;

        public override void GetBytes(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = _value++;
            }
        }
    }

    private class StringHasherOptionsAccessor : Pbkdf2HasherOptions
    {
        public Pbkdf2HasherOptions Value { get; } = new Pbkdf2HasherOptions();
    }
}