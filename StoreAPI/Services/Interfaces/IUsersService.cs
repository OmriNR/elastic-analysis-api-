using Domain;

namespace Services.Interfaces;

public interface IUsersService
{
    User GetUser(string email, string password, out Statuses status, out string error);
    User UpdateUser(User user, out Statuses status, out string error);
    User CreateUser(User user, out Statuses status, out string error);
    void SetAdmin(string target, string request, out Statuses status, out string error);
    void SetActive(string target, string request, out Statuses status, out string error);
    List<User> GetAllUsers(string userId, out Statuses status, out string error);
    User GetUserById(string id);
}