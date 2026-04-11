using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Repositories;

public class UsersPropertiesRepository : IUsersPropertiesRepository
{
    private readonly AppDBContext _context;

    public UsersPropertiesRepository(AppDBContext context)
    {
        _context = context;
    }

    public void CreateUser(UserProperties user)
    {
        _context.UsersProperties.Add(user);
        _context.SaveChanges();
    }

    public void UpdateUser(UserProperties user)
    {
        var local = _context.UsersProperties.Local.FirstOrDefault(u => u.UserId == user.UserId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }
        
        _context.UsersProperties.Update(user);
        _context.SaveChanges();
    }

    public void DeleteUser(UserProperties user)
    {
        _context.UsersProperties.Remove(user);
        _context.SaveChanges();
    }

    public UserProperties? GetUser(string userId)
    {
        var user = _context.UsersProperties.Find(userId);
        return user;
    }
}