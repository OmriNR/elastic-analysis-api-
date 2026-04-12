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
}