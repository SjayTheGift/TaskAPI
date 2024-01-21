using TaskAPI.Models;

namespace TaskAPI.Interfaces
{
    public interface IUsersInterface
    {
        ICollection<User> GetUsers();

        User GetUserByID(int userId);
        User GetUserByUsername(string username);
        User GetUserByEmail(string userEmail);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        bool VerifyPassword(string password, string storedHash);



        bool UserExists(int id);

    }
}
