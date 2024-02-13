using AutoFixture.Xunit2;
using Xunit.Sdk;

namespace DefectDojoJob.Tests.AutoDataAttribute;

public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] objects) : base(new AutoMoqDataAttribute(), objects)
    {
    }
}
