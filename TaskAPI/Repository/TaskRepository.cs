using System.Threading.Tasks;
using TaskAPI.Data;
using TaskAPI.Dto;
using TaskAPI.Interfaces;
using TaskAPI.Models;
using Task = TaskAPI.Models.Task;

namespace TaskAPI.Repository
{
    public class TaskRepository : ITasksInterface
    {
        private readonly DataContext _context;
        public TaskRepository(DataContext context) 
        {
            _context = context;
        }

        public Task GetTask(int taskId)
        {
            return _context.Tasks.Where(t => t.ID == taskId).FirstOrDefault();
        }

        public ICollection<Models.Task> GetAllTasks()
        {
            return _context.Tasks.OrderBy(t => t.ID).ToList();
        }
        public void UpdateTask(int taskId, int userId, Task updatedTask)
        {

            var task = _context.Tasks.FirstOrDefault(t => t.ID == taskId && t.UserId == userId);
            if (task != null)
            {
                _context.Tasks.Update(updatedTask);
                _context.SaveChanges();
            }
            else
            {
                // Handle the case when the task is not found or does not belong to the user
                throw new InvalidOperationException("The specified task does not exist or does not belong to the user.");
            }

          

        }

        public void DeleteTask(int taskId, int userId)
        {
            {
                var task = _context.Tasks.FirstOrDefault(t => t.ID == taskId && t.UserId == userId);

                if (task != null)
                {
                    // The task belongs to the user, delete it
                    _context.Tasks.Remove(task);
                    _context.SaveChanges();
                }
                else
                {
                    // The task does not belong to the user, throw an exception
                    throw new InvalidOperationException("The specified task does not exist or does not belong to the user.");
                }
            }
        }


        public bool TaskExists(int id)
        {
            return _context.Tasks.Any(t => t.ID == id);
        }

        public void CreateTask(Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

    }
}
