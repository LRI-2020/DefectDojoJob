using AutoFixture;
using AutoFixture.AutoMoq;

namespace DefectDojoJob.Tests.AutoDataAttribute;

public class AutoMoqDataAttribute : AutoFixture.Xunit2.AutoDataAttribute
{
      public AutoMoqDataAttribute() :
            base(() => new Fixture().Customize(new AutoMoqCustomization()))
      {
      }
}