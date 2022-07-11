using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Clinia.Cryptography.Pbkdf2;

/// <summary>
///     Implements the standard PBKDF2 string hashing.
/// </summary>
public class Pbkdf2Hasher : IHasher
{
    /* =======================
     * HASHED STRING FORMATS
     * =======================
     *
     * Version 1:
     * PBKDF2 with HMAC-SHA256, 128-bit salt, 256-bit subkey, 10000 iterations.
     * Format: { 0x01, prf (UInt32), iter count (UInt32), salt length (UInt32), salt, subkey }
     * (All UInt32s are stored big-endian.)
     */

    private readonly Pbkdf2HasherCompatibilityMode _compatibilityMode;
    private readonly int _iterCount;
    private readonly RandomNumberGenerator _rng;

    /// <summary>
    ///     Creates a new instance of <see cref="Pbkdf2Hasher{TIdentity}" />
    /// </summary>
    /// <param name="optionsAccessor">The options for this instance</param>
    public Pbkdf2Hasher(Pbkdf2HasherOptions optionsAccessor = null)
    {
        var options = optionsAccessor ?? new Pbkdf2HasherOptions();

        _compatibilityMode = options.CompatibilityMode;
        switch (_compatibilityMode)
        {
            case Pbkdf2HasherCompatibilityMode.V1:
                _iterCount = options.IterationCount;
                if (_iterCount < 1)
                    throw new InvalidOperationException(Resources.InvalidHasherIterationCount);
                break;

            default:
                throw new InvalidOperationException(Resources.InvalidHasherCompatibilityMode);
        }

        _rng = options.Rng;
    }

    /// <summary>
    ///     Returns a hashed representation of the supplied <paramref name="str" />
    /// </summary>
    /// <param name="str">The string to hash.</param>
    /// <returns>A hashed representation of the supplied <paramref name="str" /></returns>
    public string Generate(string str)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        return Convert.ToBase64String(HashStringV1(str, _rng));
    }

    /// <summary>
    ///     Returns a <see cref="HasherVerificationResult" /> indicating the result of a string hash comparison.
    /// </summary>
    /// <param name="hashedString">The hash value for a string</param>
    /// <param name="providedString">The string supplied for comparison</param>
    /// <returns>A <see cref="HasherVerificationResult" /> indicating the result of a string hash comparison</returns>
    /// <remarks>Implementations of this method should be time consistent.</remarks>
    public HasherVerificationResult Compare(string hashedString, string providedString)
    {
        if (hashedString == null) throw new ArgumentNullException(nameof(hashedString));
        if (providedString == null) throw new ArgumentNullException(nameof(providedString));

        var decodedHashedString = Convert.FromBase64String(hashedString);

        // read the format marker from the hashed string
        if (decodedHashedString.Length == 0) return HasherVerificationResult.Failed;
        switch (decodedHashedString[0])
        {
            case 0x01:
                if (VerifyHashedStringV1(decodedHashedString, providedString, out int embeddedIterCount))
                    // If this hasher was configured with a higher iteration count, change the entry now.
                    return embeddedIterCount < _iterCount
                        ? HasherVerificationResult.SuccessRehashNeeded
                        : HasherVerificationResult.Success;
                else
                    return HasherVerificationResult.Failed;

            default:
                return HasherVerificationResult.Failed; // unknown format marker
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static bool ByteArraysEqual(byte[] a, byte[] b)
    {
        if (a == null && b == null) return true;

        if (a == null || b == null || a.Length != b.Length) return false;

        var areSame = true;
        for (var i = 0; i < a.Length; i++) areSame &= a[i] == b[i];

        return areSame;
    }

    private byte[] HashStringV1(string str, RandomNumberGenerator rng)
    {
        return HashStringV1(str, rng,
            KeyDerivationPrf.HMACSHA256,
            _iterCount,
            128 / 8,
            256 / 8);
    }

    private static byte[] HashStringV1(string str, RandomNumberGenerator rng, KeyDerivationPrf prf,
        int iterCount, int saltSize, int numBytesRequested)
    {
        // Produce a version 3 (see comment above) text hash.
        var salt = new byte[saltSize];
        rng.GetBytes(salt);
        var subkey = KeyDerivation.Pbkdf2(str, salt, prf, iterCount, numBytesRequested);

        var outputBytes = new byte[13 + salt.Length + subkey.Length];
        outputBytes[0] = 0x01; // format marker
        WriteNetworkByteOrder(outputBytes, 1, (uint) prf);
        WriteNetworkByteOrder(outputBytes, 5, (uint) iterCount);
        WriteNetworkByteOrder(outputBytes, 9, (uint) saltSize);
        Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
        Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
        return outputBytes;
    }

    private static bool VerifyHashedStringV1(byte[] hashedString, string providedString, out int iterCount)
    {
        iterCount = default;

        try
        {
            // Read header information
            var prf = (KeyDerivationPrf) ReadNetworkByteOrder(hashedString, 1);
            iterCount = (int) ReadNetworkByteOrder(hashedString, 5);
            var saltLength = (int) ReadNetworkByteOrder(hashedString, 9);

            // Read the salt: must be >= 128 bits
            if (saltLength < 128 / 8) return false;
            var salt = new byte[saltLength];
            Buffer.BlockCopy(hashedString, 13, salt, 0, salt.Length);

            // Read the subkey (the rest of the payload): must be >= 128 bits
            var subkeyLength = hashedString.Length - 13 - salt.Length;
            if (subkeyLength < 128 / 8) return false;
            var expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(hashedString, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming string and verify it
            var actualSubkey = KeyDerivation.Pbkdf2(providedString, salt, prf, iterCount, subkeyLength);
            return ByteArraysEqual(actualSubkey, expectedSubkey);
        }
        catch
        {
            // This should never occur except in the case of a malformed payload, where
            // we might go off the end of the array. Regardless, a malformed payload
            // implies verification failed.
            return false;
        }
    }

    private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
    {
        return ((uint) buffer[offset + 0] << 24)
               | ((uint) buffer[offset + 1] << 16)
               | ((uint) buffer[offset + 2] << 8)
               | buffer[offset + 3];
    }

    private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
    {
        buffer[offset + 0] = (byte) (value >> 24);
        buffer[offset + 1] = (byte) (value >> 16);
        buffer[offset + 2] = (byte) (value >> 8);
        buffer[offset + 3] = (byte) (value >> 0);
    }
}