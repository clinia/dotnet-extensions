using Xunit;

namespace Clinia.Cryptography.Test;

public class SecureRandomTest
{
    [Theory]
    [InlineData(8)]
    public void ShouldGenerateRandomToken(ushort length)
    {
        var token1 = SecureRandom.NewToken(length);
        var token2 = SecureRandom.NewToken(length);
        
        // assert length
        Assert.Equal(token1.Length, length);
        Assert.Equal(token2.Length, length);
        
        // assert not equal
        Assert.NotEqual(token1, token2);
    }
}