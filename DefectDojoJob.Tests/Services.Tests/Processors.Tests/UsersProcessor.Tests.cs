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
            .ReturnsAsync(new User(username) { Id = 1 });

        var res = await sut.ProcessUserAsync(username);

        res.AssetIdentifier.Should().Be(username);
        res.DefectDojoId.Should().Be(1);
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

        await act.Should().ThrowAsync<WarningAssetProjectProcessor>()
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
            .ReturnsAsync(new User(username) { Id = 1 });

        var usernames = new List<string>() { username };

        var res = await sut.ProcessUsersAsync(usernames);

        res.Entities.Count.Should().Be(1);
        res.Entities[0].AssetIdentifier.Should().Be(username);
        res.Entities[0].DefectDojoId.Should().Be(1);
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
        res.Entities.Should().BeEmpty();
        res.Warnings.Count.Should().Be(1);
        res.Warnings[0].AssetIdentifier.Should().Be(username);
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
       res.Entities.Should().BeEmpty();
        res.Errors.Count.Should().Be(1);
        res.Errors[0].AssetIdentifier.Should().Be(username);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenSeveralUsernames_AllAreProcessed(
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(new User("username") { Id = 1});

        var itemsNumber = new Random().Next(3, 15);
        var usernames = new Fixture().CreateMany<string>(itemsNumber).ToList();

        var res = await sut.ProcessUsersAsync(usernames);
        res.Entities.Count.Should().Be(itemsNumber);
    }

    [Theory]
    [AutoMoqData]
    public async Task WhenErrorOccured_OtherEntitiesProcessed(string username1, string username2, string username3,
        [Frozen] Mock<IDefectDojoConnector> defectDojoConnectorMock,
        UsersProcessor sut)
    {
        //Arrange
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(username3))
            .ReturnsAsync(new User(username1) { Id = 1});
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(username2))
            .ReturnsAsync((User?)null);
        defectDojoConnectorMock.Setup(d => d.GetDefectDojoUserByUsernameAsync(username1))
            .Throws<Exception>();

        var usernames = new List<string>() { username1, username2, username3 };

        var res = await sut.ProcessUsersAsync(usernames);
        res.Entities.Count.Should().Be(1);
        res.Warnings.Count.Should().Be(1);
        res.Errors.Count.Should().Be(1);
    }
}