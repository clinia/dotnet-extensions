namespace Clinia.Cryptography;

/// <summary>
///     Provides an abstraction for generating and comparing string hashes.
/// </summary>
public interface IHasher
{
    /// <summary>
    ///     Returns a hashed representation of the supplied <paramref name="str" />.
    /// </summary>
    /// <param name="str">The string to hash.</param>
    /// <returns>A hashed representation of the supplied <paramref name="str" /> </returns>
    string Generate(string str);

    /// <summary>
    ///     Returns a <see cref="HasherVerificationResult" /> indicating the result of a string hash comparison.
    /// </summary>
    /// <param name="hashedString">The hash value for a string stored.</param>
    /// <param name="providedString">The string supplied for comparison.</param>
    /// <returns>A <see cref="HasherVerificationResult" /> indicating the result of a string hash comparison.</returns>
    /// <remarks>Implementations of this method should be time consistent.</remarks>
    HasherVerificationResult Compare(string hashedString, string providedString);
}