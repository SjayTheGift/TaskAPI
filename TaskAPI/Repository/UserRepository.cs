using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TaskAPI.Data;
using TaskAPI.Interfaces;
using TaskAPI.Models;

namespace TaskAPI.Repository
{
    public class UserRepository : IUsersInterface
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context) 
        {
            _context = context;
        }


        public ICollection<User> GetUsers()
        {
            return _context.Users.OrderBy(u => u.ID).ToList();
        }

        public User GetUserByID(int userId)
        {
            return _context.Users.Find(userId);
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.Where(u => u.Username == username).FirstOrDefault();
        }

        public User GetUserByEmail(string userEmail)
        {
            return _context.Users.Where(u => u.Email == userEmail).FirstOrDefault();
        }

        public User GetUserByEmailAndPassword(string userEmail, string userPassword)
        {
            return _context.Users.FirstOrDefault(u => u.Email == userEmail && u.Password == userPassword);
        }

        public bool UserExists(int id)
        {
            return _context.Users.Any(u => u.ID == id);
        }

        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

        }
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}

