using System.Security.Cryptography;

namespace Clinia.Cryptography;

public static class SecureRandom
{
    /// <summary>
    /// Generate a secure unique random token
    /// </summary>
    /// <param name="length">The desired length of the token</param>
    /// <returns>The unique token as a string</returns>
    public static string NewToken(ushort length, string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
    {
        var data = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        
        // If chars.Length isn't a power of 2 then there is a bias if we simply use the modulus operator. The first characters of chars will be more probable than the last ones.
        // buffer used if we encounter an unusable random byte. We will regenerate it in this buffer
        byte[] buffer = null;
        
        // Maximum random number that can be used without introducing a bias
        var maxRandom = byte.MaxValue - (byte.MaxValue + 1) % chars.Length;
        
        rng.GetBytes(data);
        
        var result = new char[length];
        
        for (var i = 0; i < length; i++)
        {
            var value = data[i];

            while (value > maxRandom)
            {
                buffer ??= new byte[1];

                rng.GetBytes(buffer);
                value = buffer[0];
            }

            result[i] = chars[value % chars.Length];
        }

        return new string(result);
    }
}