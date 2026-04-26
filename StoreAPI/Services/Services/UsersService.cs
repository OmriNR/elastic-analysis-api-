using Domain;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _repository;
    private readonly ILogger<UsersService> _logger;

    public UsersService(IUsersRepository repository, ILogger<UsersService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public User GetUser(string email, string password, out Statuses status, out string error)
    {
        status = Statuses.OK;
        error = string.Empty;
        
        _logger.LogInformation($"Getting user by email {email}");
        var user = _repository.GetUserByEmail(email);

        if (user == null)
        {
            _logger.LogError($"User {email} not found");
            status = Statuses.NOT_FOUND;
            error = "User not found";
            return null;
        }

        if (password != user.Password)
        {
            _logger.LogError("Passwords do not match");
            status = Statuses.INVALID;
            error = "Passwords do not match";
            return null;
        }

        return user;
    }

    public User UpdateUser(User user, out Statuses status, out string error)
    {
        status = Statuses.OK;
        error = string.Empty;
        _logger.LogInformation($"Updating user by email {user.Email}");

        if (!IsUserValid(user, out error))
        {
            status = Statuses.INVALID;
            return null;
        }

        var exists = _repository.GetUserByEmail(user.Email);

        if (exists == null)
        {
            _logger.LogError($"User {user.Email} not found");
            status = Statuses.NOT_FOUND;
            error = "User not found";
            return null;
        }
        
        _repository.UpdateUser(user);
        
        var updatedUser = _repository.GetUserByEmail(user.Email);
        
        return updatedUser;
    }

    public User CreateUser(User user, out Statuses status, out string error)
    {
        status = Statuses.OK;
        error = string.Empty;
        
        _logger.LogInformation("Creating user");

        if (!IsUserValid(user, out error))
        {
            status = Statuses.INVALID;
            return null;
        }

        var check = _repository.GetUserByEmail(user.Email);

        if (check != null)
        {
            _logger.LogError("User already exists");
            status = Statuses.INVALID;
            error = "User  already exists";
            return null;
        }

        string id = Guid.NewGuid().ToString();

        user.UserId = id;
        
        _repository.CreateUser(user);

        var newUser = _repository.GetUserById(id);

        return newUser;
    }

    public void SetAdmin(string requested, string asked, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;
        
        _logger.LogInformation("setting admin for user {0} by {1}", requested, asked);
        
        var askedUser = _repository.GetUserById(asked);
        var requestedUser = _repository.GetUserById(requested);

        if (askedUser == null || requestedUser == null)
        {
            _logger.LogError($"One of the users does not exist");
            error = "One of the users does not exist";
            status = Statuses.NOT_FOUND;
            return;
        }

        if (!askedUser.IsAdmin)
        {
            _logger.LogError("The asked user is not admin");
            error = "The asked user is not admin";
            status = Statuses.INVALID;
            return;
        }

        if (requestedUser.IsAdmin)
        {
            _logger.LogError("The requested user is already admin");
            error = "The requested user is already admin";
            status = Statuses.INVALID;
            return;
        }
        
        requestedUser.IsAdmin = true;
        _repository.UpdateUser(requestedUser);
    }

    public void SetActive(string requested, string asked, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;
        
        _logger.LogInformation("setting activity for user {0} by {1}", requested, asked);
        
        var askedUser = _repository.GetUserById(asked);
        var requestedUser = _repository.GetUserById(requested);

        if (askedUser == null || requestedUser == null)
        {
            _logger.LogError($"One of the users does not exist");
            error = "One of the users does not exist";
            status = Statuses.NOT_FOUND;
            return;
        }

        if (!askedUser.IsAdmin)
        {
            _logger.LogError("The asked user is not admin");
            error = "The asked user is not admin";
            status = Statuses.INVALID;
            return;
        }
        
        requestedUser.IsActive = !requestedUser.IsActive;
        _repository.UpdateUser(requestedUser);
    }

    public List<User> GetAllUsers(string userId, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;
        _logger.LogInformation($"Getting all users, asked user {userId}");
        
        var requestedUser = _repository.GetUserById(userId);

        if (requestedUser == null)
        {
            _logger.LogError($"User {userId} not found");
            error = "User not found";
            status = Statuses.INVALID;
            return null;
        }

        if (!requestedUser.IsAdmin)
        {
            _logger.LogError("The requested user is not admin");
            error = "The requested user is not admin";
            status = Statuses.INVALID;
            return null;
        }
        
        var allUsers = _repository.GetAllUsers();

        return allUsers;
    }

    private bool IsUserValid(User user, out string error)
    {
        error = string.Empty;
        if (user.Email == string.Empty)
        {
            error = "Email must be filled";
            return false;
        }
        
        if (user.Password == string.Empty)
        {
            error = "Password must be filled";
            return false;
        }
        
        if (user.Properties.Gender == string.Empty)
        {
            error = "Gender must be filled";
            return false;
        }
        
        if (user.Properties.Gender == string.Empty)
        {
            error = "Gender must be filled";
            return false;
        }
        
        if (user.Properties.UserName == string.Empty)
        {
            error = "UserName must be filled";
            return false;
        }

        if (user.Properties.Location.Address == string.Empty)
        {
            error = "Address must be filled";
            return false;
        }
        
        if (user.Properties.Location.City == string.Empty)
        {
            error = "City must be filled";
            return false;
        }
        
        if (user.Properties.Location.Country == string.Empty)
        {
            error = "Country must be filled";
            return false;
        }
        
        if (user.Properties.Location.ZipCode == string.Empty)
        {
            error = "ZipCode must be filled";
            return false;
        }

        return true;
    }
}