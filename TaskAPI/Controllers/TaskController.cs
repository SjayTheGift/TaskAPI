using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskAPI.Data;
using TaskAPI.Dto;
using TaskAPI.Interfaces;
using TaskAPI.Models;
using Task = TaskAPI.Models.Task;

namespace TaskAPI.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IUsersInterface _usersInterface;
        private readonly ITasksInterface _tasksInterface;
        private readonly IMapper _mapper;
        public TaskController(ITasksInterface tasksInterface, IMapper mapper, IUsersInterface usersInterface)
        {
            _tasksInterface = tasksInterface;
            _mapper = mapper;
            _usersInterface = usersInterface;
        }

        // GET All tasks api/tasks
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Task>))]
        public IActionResult GetTasks()
        {
            var users = _tasksInterface.GetAllTasks();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(users);
        }

        // GET Single tasks api/tasks/{id}
        [HttpGet("{taskId}")]
        [ProducesResponseType(200, Type = typeof(Task))]
        public IActionResult GetUserByID(int taskId)
        {
            if (!_tasksInterface.TaskExists(taskId))
            {
                return NotFound();
            }
               

            var user = _tasksInterface.GetTask(taskId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(user);
        }

        // GET Single tasks api/tasks/create
        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(Task))]
        public IActionResult CreateTask(AddTaskDto addTaskk)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return NotFound("User not found");
            }

            int userId = Convert.ToInt32(userIdClaim.Value);


            var user = _usersInterface.GetUserByID(userId);

            if (user == null)
            {
                return BadRequest("Invalid UserId");
            }


            var task = new Task
            {
                Title = addTaskk.Title,
                Description = addTaskk.Description,
                Assignee = user,
                DueDate = addTaskk.DueDate

            };
            // Save the task to the database
            _tasksInterface.CreateTask(task);

            return Ok("Task created successfully");
        }

        // Update Single tasks that belongs to logged in user api/tasks/taskId
        [HttpPut("{taskId}")]
        [ProducesResponseType(200, Type = typeof(User))]
        public IActionResult UpdateTask(int taskId, UpdateTaskDto updateTask)
        {
            var task = _tasksInterface.GetTask(taskId);
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return NotFound("User not found");
            }

            if (task == null)
            {
                return NotFound("Task not found");
            }

             int userId = Convert.ToInt32(userIdClaim.Value);

            task.Title = updateTask.Title;
            task.Description = updateTask.Description;

            _tasksInterface.UpdateTask(taskId, userId, task);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(task);
        }

        // Delete Single tasks that belongs to logged in user api/tasks/taskId
        [HttpDelete("{taskId}")]
        [ProducesResponseType(200, Type = typeof(Task))]
        public IActionResult DeleteTask(int taskId)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return NotFound("User not found");
            }

            int userId = Convert.ToInt32(userIdClaim.Value);


            var user = _usersInterface.GetUserByID(userId);

            if (user == null)
            {
                return BadRequest("Invalid UserId");
            }

            try
            {
                // delete from database
                _tasksInterface.DeleteTask(taskId, userId);
            }
            catch (InvalidOperationException ex)
            {
                // Handle the exception
                return BadRequest("Error: " + ex.Message);
            }

            return Ok("Deleted successfully");
        }

    }
}
