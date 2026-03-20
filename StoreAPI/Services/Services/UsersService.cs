using Domain;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class UsersService : IUsersService
{
    private readonly IUserRepository _userRepository;
    public UsersService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User GetUser(string userId, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;

        var user = _userRepository.GetUser(userId);

        if (user == null)
        {
            status = Statuses.NOT_FOUND;
            error = $"User {userId} not found";
            return null;
        }

        return user;
    }

    public User CreateUser(User user, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;

        if (IsUserValid(user, out error))
        {
            Guid userId = Guid.NewGuid();
            user.UserId = userId.ToString();
            user.CreatedAt = DateTime.UtcNow.ToUniversalTime();
            
            _userRepository.CreateUser(user);
            var newUser = _userRepository.GetUser(userId.ToString());
            
            return newUser!;
        }
        
        status = Statuses.INVALID;
        return null;
    }

    public User UpdeateUser(User user, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;
        
        var check = _userRepository.GetUser(user.UserId);

        if (check == null)
        {
            status = Statuses.NOT_FOUND;
            error = $"User {user.UserId} not found";
            return null;
        }

        if (IsUserValid(user, out error))
        {
            _userRepository.UpdateUser(user);
            
            var updatedUser = _userRepository.GetUser(user.UserId);
            return updatedUser!;
        }
        
        status = Statuses.INVALID;
        return null;
    }

    public void DeleteUser(string userId, out Statuses status, out string error)
    {
        status = Statuses.OK;
        error = string.Empty;
        
        var user = _userRepository.GetUser(userId);

        if (user == null)
        {
            status = Statuses.NOT_FOUND;
            error = "User {userId} not found";
        }
        else
        {
            _userRepository.DeleteUser(user);
        }
    }
    
    private bool IsUserValid(User user, out string error)
    {
        error = string.Empty;

        if (user.Age <= 0)
        {
            error += " Age must be greater than zero";
        }

        if (user.UserName == string.Empty || user.UserName == null)
        {
            error += " USERNAME is required";
        }
        
        return error == string.Empty;
    }
}