using Shared.Models;

namespace Services.Interfaces;

public interface IUsersService
{
    User GetUser(string userId, out Statuses status, out string error);
    User CreateUser(User user, out Statuses status, out string error);
    User UpdeateUser(User user, out Statuses status, out string error);
    void DeleteUser(string userId, out Statuses status, out string error);
}