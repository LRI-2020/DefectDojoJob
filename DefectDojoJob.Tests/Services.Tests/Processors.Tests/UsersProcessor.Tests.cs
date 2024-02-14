using AutoFixture.Xunit2;
using DefectDojoJob.Models.DefectDojo;
using DefectDojoJob.Models.Processor.Errors;
using DefectDojoJob.Services.Interfaces;
using DefectDojoJob.Services.Processors;
using DefectDojoJob.Tests.AutoDataAttribute;
using FluentAssertions;
using Moq;

namespace DefectDojoJob.Tests.Services.Tests.Processors.Tests;

public class UsersProcessorTests
{
    [Theory]
    [AutoMoqData]
    public async Task WhenUserNotNull_ProcessUserAsyncReturnsUserDefectDojoId(string username,
        [Frozen]Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsername(username))
            .ReturnsAsync(new User() { Id = 1, UserName = username });
        
        var res = await sut.ProcessUserAsync(username);
        
        res.AssetIdentifier.Should().Be(username);
        res.DefectDojoId.Should().Be(1);
    }
    
    [Theory]
    [AutoMoqData]
    public async Task WhenUserIsNull_ProcessUserAsyncThrowException(string username,
        [Frozen]Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsername(username))
            .ReturnsAsync((User?)null);
        
        Func<Task> act = ()=>sut.ProcessUserAsync(username);

        await act.Should().ThrowAsync<WarningAssetProjectInfoProcessor>()
            .Where(e => e.Message.Contains("user") && e.Message.Contains("does not exist"));
    }
}