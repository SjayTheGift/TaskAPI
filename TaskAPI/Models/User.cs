namespace TaskAPI.Models
{
    // User.cs
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }

}
