using Domain;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class UsersPropertiesService : IUsersPropertiesService
{
    private readonly ILogger<IUsersPropertiesService> _logger;
    private readonly IUsersPropertiesRepository _userRepository;

    public UsersPropertiesService(ILogger<IUsersPropertiesService> logger, IUsersPropertiesRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public UserProperties GetUser(string userId, out Statuses status, out string error)
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

    public UserProperties CreateUser(UserProperties user, out Statuses status, out string error)
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

    public UserProperties UpdeateUser(UserProperties user, out Statuses status, out string error)
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
    
    private bool IsUserValid(UserProperties user, out string error)
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