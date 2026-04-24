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
    private const string EXISTS_EMAIL = "exists@email.com";
    private const string NOT_EXISTS_EMAIL = "notexists@email.com";

    private const string VALID_PASSWORD = "ValidPassword";
    private const string INVALID_PASSWORD = "WrongPassword";

    private const string USER_ID = "user-id-123";

    private static User validUser = new User()
    {
        UserId = USER_ID,
        Email = EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        IsActive = true,
        Properties = new UserProperties()
        {
            UserName = "TestUser",
            Age = 25,
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "TestCity",
                Country = "TestCountry",
                ZipCode = "12345"
            }
        }
    };

    private static User validNewUser = new User()
    {
        Email = NOT_EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        Properties = new UserProperties()
        {
            UserName = "NewUser",
            Age = 25,
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "TestCity",
                Country = "TestCountry",
                ZipCode = "12345"
            }
        }
    };

    private static User userEmailAlreadyExists = new User()
    {
        Email = EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        Properties = new UserProperties()
        {
            UserName = "ExistingUser",
            Age = 25,
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "TestCity",
                Country = "TestCountry",
                ZipCode = "12345"
            }
        }
    };

    private static User userNotFound = new User()
    {
        Email = NOT_EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        Properties = new UserProperties()
        {
            UserName = "TestUser",
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "TestCity",
                Country = "TestCountry",
                ZipCode = "12345"
            }
        }
    };

    private static User userNoEmail = new User()
    {
        Email = "",
        Password = VALID_PASSWORD,
        Properties = new UserProperties()
        {
            UserName = "TestUser",
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "TestCity",
                Country = "TestCountry",
                ZipCode = "12345"
            }
        }
    };

    private static User userNoPassword = new User()
    {
        Email = EXISTS_EMAIL,
        Password = "",
        Properties = new UserProperties()
        {
            UserName = "TestUser",
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "TestCity",
                Country = "TestCountry",
                ZipCode = "12345"
            }
        }
    };

    private static User userNoGender = new User()
    {
        Email = EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        Properties = new UserProperties()
        {
            UserName = "TestUser",
            Gender = "",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "TestCity",
                Country = "TestCountry",
                ZipCode = "12345"
            }
        }
    };

    private static User userNoUserName = new User()
    {
        Email = EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        Properties = new UserProperties()
        {
            UserName = "",
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "TestCity",
                Country = "TestCountry",
                ZipCode = "12345"
            }
        }
    };

    private static User userNoAddress = new User()
    {
        Email = EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        Properties = new UserProperties()
        {
            UserName = "TestUser",
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "",
                City = "TestCity",
                Country = "TestCountry",
                ZipCode = "12345"
            }
        }
    };

    private static User userNoCity = new User()
    {
        Email = EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        Properties = new UserProperties()
        {
            UserName = "TestUser",
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "",
                Country = "TestCountry",
                ZipCode = "12345"
            }
        }
    };

    private static User userNoCountry = new User()
    {
        Email = EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        Properties = new UserProperties()
        {
            UserName = "TestUser",
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "TestCity",
                Country = "",
                ZipCode = "12345"
            }
        }
    };

    private static User userNoZipCode = new User()
    {
        Email = EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        Properties = new UserProperties()
        {
            UserName = "TestUser",
            Gender = "Male",
            Location = new GeoProperties()
            {
                Address = "123 Main St",
                City = "TestCity",
                Country = "TestCountry",
                ZipCode = ""
            }
        }
    };

    private Mock<IUsersRepository> _usersRepositoryMock;
    private IUsersService _service;

    [SetUp]
    public void Setup()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();

        _usersRepositoryMock.Setup(repo =>
            repo.GetUserByEmail(EXISTS_EMAIL)).Returns(validUser);

        _usersRepositoryMock.Setup(repo =>
            repo.GetUserByEmail(NOT_EXISTS_EMAIL)).Returns((User)null);

        _usersRepositoryMock.Setup(repo =>
            repo.GetUserById(It.IsAny<string>())).Returns(validUser);

        ILogger<UsersService> _logger = new NullLogger<UsersService>();
        _service = new UsersService(_usersRepositoryMock.Object, _logger);
    }

    [TestCase(EXISTS_EMAIL, VALID_PASSWORD, Statuses.OK, "")]
    [TestCase(NOT_EXISTS_EMAIL, VALID_PASSWORD, Statuses.NOT_FOUND, "User not found")]
    [TestCase(EXISTS_EMAIL, INVALID_PASSWORD, Statuses.INVALID, "Passwords do not match")]
    public void Get_User(string email, string password, Statuses expectedStatus, string expectedError)
    {
        var user = _service.GetUser(email, password, out var status, out var error);

        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    [Test, TestCaseSource(nameof(UpdateUserTestCases))]
    public void Update_User(User user, Statuses expectedStatus, string expectedError)
    {
        _service.UpdateUser(user, out var status, out var error);

        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    [Test, TestCaseSource(nameof(CreateUserTestCases))]
    public void Create_User(User user, Statuses expectedStatus, string expectedError)
    {
        _service.CreateUser(user, out var status, out var error);

        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    public static IEnumerable<TestCaseData> CreateUserTestCases()
    {
        yield return new TestCaseData(validNewUser, Statuses.OK, "");
        yield return new TestCaseData(userEmailAlreadyExists, Statuses.INVALID, "User  already exists");
        foreach (var tc in UserValidationTestCases()) yield return tc;
    }

    public static IEnumerable<TestCaseData> UpdateUserTestCases()
    {
        yield return new TestCaseData(validUser, Statuses.OK, "");
        yield return new TestCaseData(userNotFound, Statuses.NOT_FOUND, "User not found");
        foreach (var tc in UserValidationTestCases()) yield return tc;
    }

    private static IEnumerable<TestCaseData> UserValidationTestCases()
    {
        yield return new TestCaseData(userNoEmail, Statuses.INVALID, "Email must be filled");
        yield return new TestCaseData(userNoPassword, Statuses.INVALID, "Password must be filled");
        yield return new TestCaseData(userNoGender, Statuses.INVALID, "Gender must be filled");
        yield return new TestCaseData(userNoUserName, Statuses.INVALID, "UserName must be filled");
        yield return new TestCaseData(userNoAddress, Statuses.INVALID, "Address must be filled");
        yield return new TestCaseData(userNoCity, Statuses.INVALID, "City must be filled");
        yield return new TestCaseData(userNoCountry, Statuses.INVALID, "Country must be filled");
        yield return new TestCaseData(userNoZipCode, Statuses.INVALID, "ZipCode must be filled");
    }
}
