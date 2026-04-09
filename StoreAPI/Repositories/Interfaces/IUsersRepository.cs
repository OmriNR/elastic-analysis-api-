using Shared.Models;

namespace Repositories.Interfaces;

public interface IUserRepository
{
    void CreateUser(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);
    User? GetUser(string userId);
}