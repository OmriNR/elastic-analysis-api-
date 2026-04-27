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
    private const string ADMIN_USER_ID = "admin-user-id";
    private const string NON_ADMIN_USER_ID = "non-admin-user-id";
    private const string TARGET_USER_ID = "target-user-id";
    private const string ALREADY_ADMIN_TARGET_ID = "already-admin-target-id";
    private const string NOT_FOUND_USER_ID = "not-found-user-id";

    private static User validUser = new User()
    {
        UserId = USER_ID,
        Email = EXISTS_EMAIL,
        Password = VALID_PASSWORD,
        IsActive = true,
        IsAdmin = false,
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

    private static User adminUser = new User()
    {
        UserId = ADMIN_USER_ID,
        IsActive = true,
        IsAdmin = true
    };

    private static User nonAdminUser = new User()
    {
        UserId = NON_ADMIN_USER_ID,
        IsActive = true,
        IsAdmin = false
    };

    private static User targetUser = new User()
    {
        UserId = TARGET_USER_ID,
        IsActive = true,
        IsAdmin = false
    };

    private static User alreadyAdminTarget = new User()
    {
        UserId = ALREADY_ADMIN_TARGET_ID,
        IsActive = true,
        IsAdmin = true
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

        // Generic fallback for GetUserById — specific setups below take precedence (Moq last-wins)
        _usersRepositoryMock.Setup(repo =>
            repo.GetUserById(It.IsAny<string>())).Returns(validUser);

        _usersRepositoryMock.Setup(repo =>
            repo.GetUserById(ADMIN_USER_ID)).Returns(adminUser);

        _usersRepositoryMock.Setup(repo =>
            repo.GetUserById(NON_ADMIN_USER_ID)).Returns(nonAdminUser);

        _usersRepositoryMock.Setup(repo =>
            repo.GetUserById(TARGET_USER_ID)).Returns(targetUser);

        _usersRepositoryMock.Setup(repo =>
            repo.GetUserById(ALREADY_ADMIN_TARGET_ID)).Returns(alreadyAdminTarget);

        _usersRepositoryMock.Setup(repo =>
            repo.GetUserById(NOT_FOUND_USER_ID)).Returns((User)null);

        _usersRepositoryMock.Setup(repo =>
            repo.GetAllUsers()).Returns(new List<User> { validUser, adminUser, nonAdminUser, targetUser });

        ILogger<UsersService> logger = new NullLogger<UsersService>();
        _service = new UsersService(_usersRepositoryMock.Object, logger);
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

    [TestCase(TARGET_USER_ID, ADMIN_USER_ID, Statuses.OK, "")]
    [TestCase(TARGET_USER_ID, NOT_FOUND_USER_ID, Statuses.NOT_FOUND, "One of the users does not exist")]
    [TestCase(TARGET_USER_ID, NON_ADMIN_USER_ID, Statuses.INVALID, "The asked user is not admin")]
    [TestCase(ALREADY_ADMIN_TARGET_ID, ADMIN_USER_ID, Statuses.INVALID, "The requested user is already admin")]
    public void Set_Admin(string target, string requestedBy, Statuses expectedStatus, string expectedError)
    {
        _service.SetAdmin(target, requestedBy, out var status, out var error);

        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    [TestCase(TARGET_USER_ID, ADMIN_USER_ID, Statuses.OK, "")]
    [TestCase(NOT_FOUND_USER_ID, ADMIN_USER_ID, Statuses.NOT_FOUND, "One of the users does not exist")]
    [TestCase(TARGET_USER_ID, NOT_FOUND_USER_ID, Statuses.NOT_FOUND, "One of the users does not exist")]
    [TestCase(TARGET_USER_ID, NON_ADMIN_USER_ID, Statuses.INVALID, "The asked user is not admin")]
    public void Set_Active(string target, string requestedBy, Statuses expectedStatus, string expectedError)
    {
        _service.SetActive(target, requestedBy, out var status, out var error);

        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    [TestCase(ADMIN_USER_ID, Statuses.OK, "")]
    [TestCase(NOT_FOUND_USER_ID, Statuses.INVALID, "User not found")]
    [TestCase(NON_ADMIN_USER_ID, Statuses.INVALID, "The requested user is not admin")]
    public void Get_All_Users(string requestedBy, Statuses expectedStatus, string expectedError)
    {
        var users = _service.GetAllUsers(requestedBy, out var status, out var error);

        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));

        if (expectedStatus == Statuses.OK)
            Assert.That(users, Is.Not.Null.And.Not.Empty);
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
