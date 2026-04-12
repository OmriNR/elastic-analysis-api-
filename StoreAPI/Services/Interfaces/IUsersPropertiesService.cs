using Domain;

namespace Services.Interfaces;

public interface IUsersPropertiesService
{
    UserProperties GetUser(string userId, out Statuses status, out string error);
    UserProperties CreateUser(UserProperties user, out Statuses status, out string error);
    UserProperties UpdeateUser(UserProperties user, out Statuses status, out string error);
    void DeleteUser(string userId, out Statuses status, out string error);
}