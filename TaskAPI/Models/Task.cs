namespace TaskAPI.Models
{
    // Task.cs
    public class Task
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public User Assignee { get; set; }
        public DateTime DueDate { get; set; }
    }
}
