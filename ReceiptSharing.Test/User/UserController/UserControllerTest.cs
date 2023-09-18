using ReceiptSharing.Api.Controllers;
using ReceiptSharing.Api.Repositories;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Test.TestUtils.Constants;
using ReceiptSharing.Test.CreateUser.TestUtils;
using ReceiptSharing.Api.Mappings;
using AutoMapper;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace ReceiptSharing.Test{

    public class UserControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;

        public UserControllerTest(ITestOutputHelper testOutputHelper)
        {

            _testOutputHelper = testOutputHelper;
            _userRepository = Substitute.For<IUserRepository>();
            _logger = Substitute.For<ILogger<AuthController>>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = configuration.CreateMapper();
            
        }
        [Fact]
        public void HandleCreateGoogleAuthLink_WhenEverythingIsCorrect_ReturnLink()
        {
            var controller = new AuthController(_userRepository, _logger, _mapper);
            // Act
            var result = controller.GoogleLogin();

            // Assert
            Assert.IsType<ChallengeResult>(result);
        }

        [Fact]
        public  void HandleCreateDiscordAuthLink_WhenEverythingIsCorrect_ReturnLink()
        {
            var controller = new AuthController(_userRepository, _logger, _mapper);
            // Act
            var result = controller.DiscordLogin();

            // Assert
            Assert.IsType<ChallengeResult>(result);
        }

        [Fact]
        public async Task HandleAuthDiscordUser_WhenClaimsExistAndUserExists_ReturnRedirect()
        {
            // Arrange
            var claims = CreateUserClaims.CreateClaims();          

            var httpContext = AuthenticationHttpContext.CreateHttpContextWithClaims(claims);
            
            _userRepository.GetUserByEmailAsync(Arg.Any<String>()).Returns((User?)null);
            var controller = new AuthController(_userRepository, _logger, _mapper);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            // Act
            var result = await controller.DiscordResponse();
   
            // Assert
            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task HandleAuthDiscordUser_WhenClaimsExistAndUserDoesntExists_ReturnRedirect()
        {
            // Arrange
            var claims = CreateUserClaims.CreateClaims();          

            var httpContext = AuthenticationHttpContext.CreateHttpContextWithClaims(claims);

            var user = UserUtils.CreateUserFromClaims(claims);
            
            _userRepository.GetUserByEmailAsync(claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value).Returns(user);
            var controller = new AuthController(_userRepository, _logger, _mapper);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            // Act
            var result = await controller.DiscordResponse();
   
            // Assert
            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task HandleAuthDiscordUser_WhenClaimsDoesntExist_ReturnBadRequest()
        {
            var httpContext = AuthenticationHttpContext.CreateHttpContextWithoutClaims();

            var controller = new AuthController(_userRepository, _logger, _mapper)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            var result = await controller.DiscordResponse();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

        }

        [Fact]
        public async Task HandleAuthGoogleUser_WhenClaimsExistAndUserDoesntExists_ReturnRedirect()
        {
            // Arrange
            var claims = CreateUserClaims.CreateClaims();          

            var httpContext = AuthenticationHttpContext.CreateHttpContextWithClaims(claims);
            
            _userRepository.GetUserByEmailAsync(claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value).Returns((User?)null);
            var controller = new AuthController(_userRepository, _logger, _mapper);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            // Act
            var result = await controller.GoogleResponse();
   
            // Assert
            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task HandleAuthGoogleUser_WhenClaimsExistAndUserExists_ReturnRedirect()
        {
            // Arrange
            var claims = CreateUserClaims.CreateClaims();          

            var httpContext = AuthenticationHttpContext.CreateHttpContextWithClaims(claims);

            var user = UserUtils.CreateUserFromClaims(claims);
            
            _userRepository.GetUserByEmailAsync(claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value).Returns(user);
            var controller = new AuthController(_userRepository, _logger, _mapper);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            // Act
            var result = await controller.GoogleResponse();
   
            // Assert
            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task HandleAuthGoogleUser_WhenClaimsDoesntExist_ReturnBadRequest()
        {
            var httpContext = AuthenticationHttpContext.CreateHttpContextWithoutClaims();

            var controller = new AuthController(_userRepository, _logger, _mapper)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            var result = await controller.GoogleResponse();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

        }

    }
}