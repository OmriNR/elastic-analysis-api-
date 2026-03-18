using Domain;

namespace Repositories.Interfaces;

public interface IUserRepository
{
    void CreateCustomer(User user);
    void UpdateCustomer(User user);
    void DeleteCustomer(User user);
    User GetCustomer(string customerId);
}