using Domain;

namespace Services.Interfaces;

public interface IUsersService
{
    User GetUser(string email, string password, out Statuses status, out string error);
    User UpdateUser(User user, out Statuses status, out string error);
    User CreateUser(User user, out Statuses status, out string error);
}