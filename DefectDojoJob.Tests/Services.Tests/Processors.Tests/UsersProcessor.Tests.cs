using AutoFixture;
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
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(username))
            .ReturnsAsync(new User() { Id = 1, UserName = username });

        var res = await sut.ProcessUserAsync(username);

        res.Entity.AssetIdentifier.Should().Be(username);
        res.Entity.DefectDojoId.Should().Be(1);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenUserIsNull_ProcessUserAsyncThrowException(string username,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(username))
            .ReturnsAsync((User?)null);

        Func<Task> act = () => sut.ProcessUserAsync(username);

        await act.Should().ThrowAsync<WarningAssetProjectInfoProcessor>()
            .Where(e => e.Message.Contains("user") && e.Message.Contains("does not exist"));
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenUserProcessingOk_UserFoundAddedToResultEntities(string username,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(username))
            .ReturnsAsync(new User() { Id = 1, UserName = username });

        var usernames = new List<string>() { username };

        var res = await sut.ProcessUsersAsync(usernames);

        var entities = res.Select(r => r.Entity).ToList();
        entities.Count.Should().Be(1);
        entities[0].AssetIdentifier.Should().Be(username);
        entities[0].DefectDojoId.Should().Be(1);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenWarning_WarningAddedToResult(string username,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(username))
            .ReturnsAsync((User?)null);
        var usernames = new List<string>() { username };

        var res = await sut.ProcessUsersAsync(usernames);
        var entities = res.Where(r=>r.Entity!=null).Select(r => r.Entity).ToList();
        var warnings = res.SelectMany(r => r.Warnings).ToList();

        entities.Should().BeEmpty();
        warnings.Count.Should().Be(1);
        warnings[0].AssetIdentifier.Should().Be(username);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenError_ErrorAddedToResult(string username,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(It.IsAny<string>()))
            .Throws<Exception>();
        var usernames = new List<string>() { username };

        var res = await sut.ProcessUsersAsync(usernames);
        var entities = res.Where(r=>r.Entity!=null).Select(r => r.Entity).ToList();
        var errors = res.SelectMany(r => r.Errors).ToList();
        entities.Should().BeEmpty();
        errors.Count.Should().Be(1);
        errors[0].AssetIdentifier.Should().Be(username);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenSeveralUsernames_AllAreProcessed(
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(new User() { Id = 1, UserName = "username" });

        var itemsNumber = new Random().Next(3, 15);
        var usernames = new Fixture().CreateMany<string>(itemsNumber).ToList();

        var res = await sut.ProcessUsersAsync(usernames);
        var entities = res.Select(r => r.Entity).ToList();
        entities.Count.Should().Be(itemsNumber);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenErrorOccured_OtherEntitiesProcessed(string username1, string username2, string username3,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(username3))
            .ReturnsAsync(new User() { Id = 1, UserName = username1 });
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(username2))
            .ReturnsAsync((User?)null);
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(username1))
            .Throws<Exception>();

        var usernames = new List<string>() { username1, username2, username3 };

        var res = await sut.ProcessUsersAsync(usernames);
        var entities = res.Where(r=>r.Entity!=null).Select(r => r.Entity).ToList();
        var warnings = res.SelectMany(r => r.Warnings).ToList();
        var errors = res.SelectMany(r => r.Errors).ToList();
        entities.Count.Should().Be(1);
        warnings.Count.Should().Be(1);
        errors.Count.Should().Be(1);
    }
}