using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Repositories;

public class UsersRepository : IUserRepository
{
    private readonly AppDBContext _context;

    public UsersRepository(AppDBContext context)
    {
        _context = context;
    }

    public void CreateUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void UpdateUser(User user)
    {
        var local = _context.Users.Local.FirstOrDefault(u => u.UserId == user.UserId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }
        
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void DeleteUser(User user)
    {
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    public User? GetUser(string userId)
    {
        var user = _context.Users.Find(userId);
        return user;
    }
}