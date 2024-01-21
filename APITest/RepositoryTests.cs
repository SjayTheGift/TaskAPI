using AutoFixture;
using Moq;
using TaskAPI.Interfaces;
using TaskAPI.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using TaskAPI.Dto;
using TaskAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace APITest
{
    [TestClass]
    public class RepositoryTests
    {
        private Fixture _fixture;
        private  Mock<IUsersInterface> _usersInterfaceMock;
        private  Mock<IMapper> _mapper;
        private  Mock<IConfiguration> _configuration;
        private UserController _userController;


        public RepositoryTests()
        {

            _fixture = new Fixture();
            _mapper = new Mock<IMapper>();
            _configuration = new Mock<IConfiguration>();
            _usersInterfaceMock = new Mock<IUsersInterface>();
            _userController = new UserController(_usersInterfaceMock.Object, _mapper.Object, _configuration.Object);
        }

        [TestMethod]
        public void UserContoller_GetUser_ReturnOK()
        {
            //Arrange
            var userList = _fixture.CreateMany<UserDto>(3).ToList();
       
            _usersInterfaceMock.Setup(repo => repo.GetUsers());

            //Act
            var result = _userController.GetUser();
            var obj = result as ObjectResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, obj.StatusCode);
        }

        [TestMethod]
        public void UserContoller_Register_ReturnOK()
        {
            // Arrange

            var user = new RegisterDto
            {
                Username = "John Doe",
                Email = "johndoe@example.com",
                Password = "test123"
            };

           
            // Act
            var result = _userController.Register(user);
            var obj = result as ObjectResult;

            // Assert
            Assert.AreEqual(200, obj.StatusCode);
            Assert.AreEqual("User registered successfully", obj.Value);
        }

        [TestMethod]
        public void UserContoller_UpdateUser_ReturnUpdatedUser()
        {
            // Arrange
            var userId = 1;
            var updatedUser = new RegisterDto { Email = "update Email", Username = "Updated User", Password = "test123" };
            _usersInterfaceMock.Setup(r => r.GetUserByID(userId)).Returns(new User { ID = userId, Username = "Old User" });

            // Act
            var result = _userController.UpdateUser(userId, updatedUser);
            var obj = result as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, obj.StatusCode);
        }

        [TestMethod]
        public void UserContoller_DeletedUser_ReturnSuccessfully()
        {

            var user = new RegisterDto
            {
                Username = "John Doe",
                Email = "johndoe@example.com",
                Password = "test123"
            };


            // Act
            var userId = 1;
            var register_user = _userController.Register(user);
            _usersInterfaceMock.Setup(r => r.GetUserByID(userId)).Returns(new User { ID = userId, Username = "Old User" });
            // Arrange


            // Act
            var result = _userController.DeleteUser(userId);
            var obj = result as ObjectResult;

            // Assert
            Assert.AreEqual(200, obj.StatusCode);
            Assert.AreEqual("User deleted successfully", obj.Value);
        }


    }
}
