using System;
using Clinia.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class HostEnvironmentExtension
    {
        public static bool IsStub(this IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(CliniaEnvironments.Stub);
        }
    }
}