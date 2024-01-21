using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskAPI.Dto;
using TaskAPI.Interfaces;
using TaskAPI.Models;
using TaskAPI.Repository;

namespace TaskAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersInterface _usersInterface;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public UserController(IUsersInterface usersInterface, IMapper mapper, IConfiguration configuration)
        {
            _usersInterface = usersInterface;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register(RegisterDto user)
        {
            // User registration logic

            // Check if the username is already taken
            if (_usersInterface.GetUserByUsername(user.Username) != null)
            {
                return BadRequest("Username is already taken");
            }

            // Check if the email is already registered
            if (_usersInterface.GetUserByEmail(user.Email) != null)
            {
                return BadRequest("Email is already registered");
            }

            // Hash the password using bcrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Create a new user object with the hashed password
            User newUser = new User
            {
                Username = user.Username,
                Email = user.Email,
                Password = hashedPassword
            };

            // Save the user to the database
            _usersInterface.CreateUser(newUser);

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDto loginRequest)
        {

            // User login logic
            var user = _usersInterface.GetUserByEmail(loginRequest.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email");
            }

            //verify hashed password
            
            bool isPasswordValid = _usersInterface.VerifyPassword(loginRequest.Password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("Invalid password");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        // GET api/users
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public IActionResult GetUser()
        {
            var users = _mapper.Map<List<UserDto>>(_usersInterface.GetUsers());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(users);
        }

        // GET Single user api/users/{id}
        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(User))]
        public IActionResult GetUserByID(int userId)
        {
            if (!_usersInterface.UserExists(userId))
            {
                return NotFound();
            }
               

            var user = _mapper.Map<UserDto>(_usersInterface.GetUserByID(userId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(user);
        }

        // Update Single user api/users/{id}
        [HttpPut("{userId}")]
        [ProducesResponseType(200, Type = typeof(User))]
        public IActionResult UpdateUser(int userId, RegisterDto updateRequest)
        {
            var user = _usersInterface.GetUserByID(userId);


            // Check if the username is already taken
            if (_usersInterface.GetUserByUsername(updateRequest.Username) != null)
            {
                return BadRequest("Username is already taken");
            }

            // Check if the email is already registered
            if (_usersInterface.GetUserByEmail(updateRequest.Email) != null)
            {
                return BadRequest("Email is already registered");
            }

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Hash the password using bcrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(updateRequest.Password);

            user.Username = updateRequest.Username;
            user.Email = updateRequest.Email;
            user.Password = hashedPassword;

            _usersInterface.UpdateUser(user);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(user);
        }

        // Delete Single user api/users/{id}
        [HttpDelete("{userId}")]
        [ProducesResponseType(200, Type = typeof(User))]
        public IActionResult DeleteUser(int userId)
        {
            var user = _usersInterface.GetUserByID(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }


            _usersInterface.DeleteUser(user);

            return Ok("User deleted successfully");
        }

    }
}
