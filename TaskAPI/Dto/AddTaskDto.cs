using TaskAPI.Models;

namespace TaskAPI.Dto
{
    public class AddTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}
