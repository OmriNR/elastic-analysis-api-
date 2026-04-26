using System.Runtime.Serialization;
using Domain;

namespace Repositories.Interfaces;

public interface IUsersRepository
{
    User GetUserById(string id);
    User GetUserByEmail(string email);
    void UpdateUser(User user);
    void CreateUser(User user);
    List<User> GetAllUsers();
}