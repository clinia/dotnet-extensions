using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Xunit;

namespace Hosting.Test
{
    public class HostEnvironmentExtensionTest
    {
        [Theory]
        [InlineData("Stub", true)]
        [InlineData("stub", true)]
        [InlineData("Development", false)]
        public void IHostEnvironment_IsStub(string value, bool expected)
        {
            IHostEnvironment hostEnvironment = new HostingEnvironment
            {
                EnvironmentName = value
            };
            
            var actual = hostEnvironment.IsStub();
            
            actual.Should().Be(expected);
        }
    }
}