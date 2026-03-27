using Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Services;

namespace TestProject1;

public class UsersTests
{
    private const string USER_ID_EXIST = "1";
    private const string USER_ID_NOT_FOUND = "2";

    private const string USER_NAME = "USERNAME";
    private const int USER_AGE = 1;

    private static User validUser = new User
    {
        UserId = USER_ID_EXIST,
        UserName = USER_NAME,
        Age = USER_AGE
    };

    private static User user_no_name = new User
    {
        UserId = USER_ID_EXIST,
        UserName = ""
    };

    private static User user_no_age = new User
    {
        UserId = USER_ID_EXIST,
        UserName = USER_NAME,
        Age = 0
    };

    private static User no_exist = new User
    {
        UserId = USER_ID_NOT_FOUND,
    };
    
    private Mock<IUserRepository> _userRepoMock;
    private IUsersService _service;
    
    [SetUp]
    public void Setup()
    {
        _userRepoMock = new Mock<IUserRepository>();
        
        _userRepoMock.Setup(repo =>
            repo.GetUser(It.Is<string>(id => id != USER_ID_NOT_FOUND))).Returns(validUser);
        
        _userRepoMock.Setup(repo =>
            repo.GetUser(It.Is<string>(id => id == USER_ID_NOT_FOUND))).Returns((User)null);
        
        ILogger<IUsersService> _logger = new NullLogger<IUsersService>();
        _service = new UsersService(_logger, _userRepoMock.Object);
    }

    [Test]
    public void Get_User_success()
    {
        var expectedStatus = Statuses.OK;
        var expectedError = "";

        var user = _service.GetUser(USER_ID_EXIST, out var status, out var error);
        
        Assert.That(user, Is.Not.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Get_User_not_found()
    {
        var expectedStatus = Statuses.NOT_FOUND;
        var expectedError = $"User {USER_ID_NOT_FOUND} not found";

        var user = _service.GetUser(USER_ID_NOT_FOUND, out var status, out var error);
        
        Assert.That(user, Is.Null);
        Assert.That(status,  Is.EqualTo(expectedStatus));
        Assert.That(error,   Is.EqualTo(expectedError));
    }

    [Test]
    public void Delete_User_not_found()
    {
        var expectedStatus = Statuses.NOT_FOUND;
        var expectedError = $"User {USER_ID_NOT_FOUND} not found";
        
        _service.DeleteUser(USER_ID_NOT_FOUND, out var status, out var error);
        
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Delete_User_success()
    {
        var expectedStatus = Statuses.OK;
        var expectedError = string.Empty;
        
        _service.DeleteUser(USER_ID_EXIST, out var status, out var error);
        
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    [Test, TestCaseSource(nameof(CreateUserTestCases))]
    public void Create_User(User user, Statuses expectedStatus, string expectedError)
    {
        _service.CreateUser(user, out var status, out var error);
        
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,   Is.EqualTo(expectedError));
    }
    
    [Test, TestCaseSource(nameof(UpdateUserTestCases))]
    public void Update_User(User user, Statuses expectedStatus, string expectedError)
    {
        _service.UpdeateUser(user, out var status, out var error);
        
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,   Is.EqualTo(expectedError));
    }

    public static IEnumerable<TestCaseData> UpdateUserTestCases()
    {
        yield return new TestCaseData(
            user_no_name,
            Statuses.INVALID,
            "USERNAME is required");
        
        yield return new TestCaseData(
            user_no_age,
            Statuses.INVALID,
            "Age must be greater than zero");

        yield return new TestCaseData(
            no_exist,
            Statuses.NOT_FOUND,
            $"User {USER_ID_NOT_FOUND} not found");
        
        yield return new TestCaseData(
            validUser,
            Statuses.OK,
            "");
    }
    
    public static IEnumerable<TestCaseData> CreateUserTestCases()
    {
        yield return new TestCaseData(
            user_no_name,
            Statuses.INVALID,
            "USERNAME is required");
        
        yield return new TestCaseData(
            user_no_age,
            Statuses.INVALID,
            "Age must be greater than zero");
        
        yield return new TestCaseData(
            validUser,
            Statuses.OK,
            "");
    }
}