using Domain;
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

    private User validUser = new User
    {
        UserId = USER_ID_EXIST,
        UserName = USER_NAME,
        Age = USER_AGE
    };

    private User user_no_name = new User
    {
        UserId = USER_ID_EXIST,
        UserName = ""
    };

    private User user_no_age = new User
    {
        UserId = USER_ID_EXIST,
        UserName = USER_NAME,
        Age = 0
    };

    private User no_exist = new User
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
        
        _service = new UsersService(_userRepoMock.Object);
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
    public void Create_User_name_invalid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError = "USERNAME is required";
        
        var user = _service.CreateUser(user_no_name, out var status, out var error );
        Assert.That(user, Is.Null);
        Assert.That(status,  Is.EqualTo(expectedStatus));
        Assert.That(error,   Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Create_User_age_invalid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError = "Age must be greater than zero";
        
        var user = _service.CreateUser(user_no_age, out var status, out var error );
        Assert.That(user, Is.Null);
        Assert.That(status,  Is.EqualTo(expectedStatus));
        Assert.That(error,   Is.EqualTo(expectedError));
    }

    [Test]
    public void Update_User_not_found()
    {
        var expectedStatus = Statuses.NOT_FOUND;
        var expectedError = $"User {USER_ID_NOT_FOUND} not found";
        
        var user = _service.UpdeateUser(no_exist, out var status, out var error);
        
        Assert.That(user, Is.Null);
        Assert.That(status,  Is.EqualTo(expectedStatus));
        Assert.That(error,   Is.EqualTo(expectedError));
    }
    [Test]
    public void Create_User_success()
    {
        var expectedStatus = Statuses.OK;
        var expectedError = string.Empty;
        
        var user = _service.CreateUser(validUser, out var status, out var error);
        
        Assert.That(user, Is.Not.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,   Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Update_User_name_invalid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError = "USERNAME is required";
        
        var user = _service.UpdeateUser(user_no_name, out var status, out var error );
        Assert.That(user, Is.Null);
        Assert.That(status,  Is.EqualTo(expectedStatus));
        Assert.That(error,   Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Update_User_age_invalid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError = "Age must be greater than zero";
        
        var user = _service.UpdeateUser(user_no_age, out var status, out var error );
        Assert.That(user, Is.Null);
        Assert.That(status,  Is.EqualTo(expectedStatus));
        Assert.That(error,   Is.EqualTo(expectedError));
    }

    [Test]
    public void Update_User_success()
    {
        var expectedStatus = Statuses.OK;
        var expectedError = string.Empty;
        
        var user = _service.UpdeateUser(validUser, out var status, out var error);
        
        Assert.That(user, Is.Not.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
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
}