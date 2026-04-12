using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDBContext _context;

    public UsersRepository(AppDBContext context)
    {
        _context = context;
    }

    public User GetUserById(string id)
    {
        var user = _context.Users.Find(id);
        
        return  user;
    }

    public User GetUserByEmail(string email)
    {
        var user = _context.Users.Where(u => u.Email == email).FirstOrDefault();
        
        return user;
    }

    public void CreateUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }
    
    public void UpdateUser(User user)
    {
        var local =  _context.Users.Where(u => u.Email == user.Email).FirstOrDefault();

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }
        
        _context.Users.Update(user);
        _context.SaveChanges();
    }
}