namespace Clinia.Cryptography;

/// <summary>
///     Specifies the results for string verification.
/// </summary>
public enum HasherVerificationResult
{
    /// <summary>
    ///     Indicates string verification failed.
    /// </summary>
    Failed = 0,

    /// <summary>
    ///     Indicates string verification was successful.
    /// </summary>
    Success = 1,

    /// <summary>
    ///     Indicates string verification was successful however the string was encoded using a deprecated algorithm
    ///     and should be rehashed and updated.
    /// </summary>
    SuccessRehashNeeded = 2
}