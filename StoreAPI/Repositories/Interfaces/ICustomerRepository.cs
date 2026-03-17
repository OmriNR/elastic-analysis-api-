using Domain;

namespace Repositories.Interfaces;

public interface ICustomerRepository
{
    void CreateCustomer(Customer customer);
    void UpdateCustomer(Customer customer);
    void DeleteCustomer(Customer customer);
    Customer GetCustomer(string customerId);
}