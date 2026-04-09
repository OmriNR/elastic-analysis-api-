using Shared.Models;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class UsersService : IUsersService
{
    private readonly ILogger<IUsersService> _logger;
    private readonly IUserRepository _userRepository;

    public UsersService(ILogger<IUsersService> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public User GetUser(string userId, out Statuses status, out string error)
    {
        _logger.LogInformation("Getting user {userId}", userId);
        error = string.Empty;
        status = Statuses.OK;

        var user = _userRepository.GetUser(userId);

        if (user == null)
        {
            _logger.LogError("User {userId} not found", userId);
            status = Statuses.NOT_FOUND;
            error = $"User {userId} not found";
            return null;
        }

        return user;
    }

    public User CreateUser(User user, out Statuses status, out string error)
    {
        _logger.LogInformation("Creating user {userId}", user.UserId);
        error = string.Empty;
        status = Statuses.OK;

        if (IsUserValid(user, out error))
        {
            _logger.LogInformation("User is valid, let's create a new user");
            Guid userId = Guid.NewGuid();
            user.UserId = userId.ToString();
            user.CreatedAt = DateTime.UtcNow.ToUniversalTime();
            
            _userRepository.CreateUser(user);
            var newUser = _userRepository.GetUser(userId.ToString());
            
            _logger.LogInformation($"User created successfully. User Id: {newUser!.UserId}");
            return newUser!;
        }
        
        _logger.LogError("User is invalid. {error}", error);
        status = Statuses.INVALID;
        return null;
    }

    public User UpdeateUser(User user, out Statuses status, out string error)
    {
        _logger.LogInformation("Updating user {userId}", user.UserId);
        error = string.Empty;
        status = Statuses.OK;
        
        var check = _userRepository.GetUser(user.UserId);

        if (check == null)
        {
            _logger.LogError("User {userId} not found", user.UserId);
            status = Statuses.NOT_FOUND;
            error = $"User {user.UserId} not found";
            return null;
        }

        if (IsUserValid(user, out error))
        {
            _logger.LogInformation("User is valid, let's update a user");
            _userRepository.UpdateUser(user);
            
            var updatedUser = _userRepository.GetUser(user.UserId);
            return updatedUser!;
        }
        
        _logger.LogError("User is invalid. {error}", error);
        status = Statuses.INVALID;
        return null;
    }

    public void DeleteUser(string userId, out Statuses status, out string error)
    {
        _logger.LogInformation("Deleting user {userId}", userId);
        status = Statuses.OK;
        error = string.Empty;
        
        var user = _userRepository.GetUser(userId);

        if (user == null)
        {
            _logger.LogError("User {userId} not found", userId);
            status = Statuses.NOT_FOUND;
            error = $"User {userId} not found";
        }
        else
        {
            _userRepository.DeleteUser(user);
        }
    }
    
    private bool IsUserValid(User user, out string error)
    {
        error = string.Empty;
        
        if (user.UserName == string.Empty || user.UserName == null)
        {
            error = "USERNAME is required";
            return false;
        }
        
        if (user.Age <= 0)
        {
            error = "Age must be greater than zero";
            return false;
        }
        
        return true;
    }
}