using ReceiptSharing.Api.Controllers;
using ReceiptSharing.Api.Repositories;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Api.Mappings;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using ReceiptSharing.Test.TestUtils;
using ReceiptSharing.Test.CreateObject.Utils;
using FluentAssertions;

namespace ReceiptSharing.Test{

    public class SubscriptionUserControllerTest
    {
        private readonly ISubscriptionUserRepository _subscriptionRepository;
        private readonly ILogger<SubscriptionUserController> _logger;
        private readonly SubscriptionUserController _controller;
        private readonly IMapper _mapper;

        public SubscriptionUserControllerTest()
        {
            _subscriptionRepository = Substitute.For<ISubscriptionUserRepository>();
            _logger = Substitute.For<ILogger<SubscriptionUserController>>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = configuration.CreateMapper();

            _controller = new SubscriptionUserController(_subscriptionRepository, _logger, _mapper);

            var user = CreateRandomUser.CreateUser();
            var httpContext = new DefaultHttpContext();

            UserInHttpContext.SetUserInHttpContext(user, httpContext);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async void SubscribingToUser_WithCorrectData_ReturnsOk()
        {
            // Arrange
            var userId = 123;
            var subscription = CreateRandomSubscription.CreateSubscription();
            _subscriptionRepository.SubscribeToUserAsync(Arg.Any<SubscriptionUser>()).Returns(subscription);
            // Act
            var result = await _controller.SubscribeToUserAsync(userId);

            // Assert
             Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void SubscribingToUser_WithWrongForgeinKey_ReturnsNotFound()
        {
            // Arrange
            var userId = 123;
            _subscriptionRepository.SubscribeToUserAsync(Arg.Any<SubscriptionUser>()).Returns((SubscriptionUser)null);
            // Act
            var result = await _controller.SubscribeToUserAsync(userId);

            // Assert
             Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async void UnSubscribingToUser_WithCorrectData_ReturnsOk()
        {
            // Arrange
            var userId = 123;
            _subscriptionRepository.UnsubscribeFromUserAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(true);
            // Act
            var result = await _controller.UnsubscribeFromUserAsync(userId);

            // Assert
             Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void UnSubscribingToUser_WithWrongForgeinKey_ReturnsNotFound()
        {
            // Arrange
            var userId = 123;
            _subscriptionRepository.UnsubscribeFromUserAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(false);
            // Act
            var result = await _controller.UnsubscribeFromUserAsync(userId);

            // Assert
             Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetUserSubscriptionsAsync_WithCorrectData_ReturnsOkResultWithSubscriptionsDto()
        {
            // Arrange
            var subscriptions = new List<SubscriptionUser>
            {
                CreateRandomSubscription.CreateSubscription(),
                CreateRandomSubscription.CreateSubscription()
            };

            _subscriptionRepository.GetUserSubscriptionsAsync(Arg.Any<int>()).Returns(subscriptions);

            // Act
            var result = await _controller.GetUserSubscriptionsAsync();

            // Assert
            Assert.IsType<ActionResult<List<SubscriptionUserDto>>>(result);         
        }


    }
}