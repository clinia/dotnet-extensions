using System.Security.Cryptography;

namespace Clinia.Cryptography.Pbkdf2;

/// <summary>
///     Specifies options for string hashing.
/// </summary>
public class Pbkdf2HasherOptions
{
    private static readonly RandomNumberGenerator _defaultRng = RandomNumberGenerator.Create(); // secure PRNG

    /// <summary>
    ///     Gets or sets the compatibility mode used when hashing strings.
    /// </summary>
    /// <value>
    ///     The compatibility mode used when hashing string.
    /// </value>
    /// <remarks>
    ///     The default compatibility mode is 'version 1'.
    /// </remarks>
    public Pbkdf2HasherCompatibilityMode CompatibilityMode { get; set; } = Pbkdf2HasherCompatibilityMode.V1;

    /// <summary>
    ///     Gets or sets the number of iterations used when hashing strings using PBKDF2.
    /// </summary>
    /// <value>
    ///     The number of iterations used when hashing strings using PBKDF2.
    /// </value>
    /// <remarks>
    ///     This value is only used when the compatibility mode is set to 'V1'.
    ///     The value must be a positive integer. The default value is 10,000.
    /// </remarks>
    public int IterationCount { get; set; } = 10000;

    // for unit testing
    internal RandomNumberGenerator Rng { get; set; } = _defaultRng;
}