using Domain;

namespace Repositories.Interfaces;

public interface IUsersPropertiesRepository
{
    void CreateUser(UserProperties user);
    void UpdateUser(UserProperties user);
    void DeleteUser(UserProperties user);
    UserProperties? GetUser(string userId);
}