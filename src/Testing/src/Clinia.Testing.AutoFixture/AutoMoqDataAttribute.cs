using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Clinia.Testing.AutoFixture
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(() => CustomizeFixture(new Fixture()))
        {}

        public AutoMoqDataAttribute(Func<IFixture> fixtureFactory) : base(() =>
            {
                var fixture = fixtureFactory.Invoke();
                return CustomizeFixture(fixture);
            })
        {}

        private static IFixture CustomizeFixture(IFixture fixture)
        {
            fixture.Customize(new AutoMoqCustomization());
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}