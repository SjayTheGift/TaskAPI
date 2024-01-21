using TaskAPI.Dto;
using TaskAPI.Models;
using Task = TaskAPI.Models.Task;

namespace TaskAPI.Interfaces
{
    public interface ITasksInterface
    {
        ICollection<Task> GetAllTasks();

        Task GetTask(int taskId);

        bool TaskExists(int id);
        void UpdateTask(int taskId, int userId, Task task);
        void DeleteTask(int taskId, int userId);
        void CreateTask(Task task);
    }
}
