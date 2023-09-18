using ReceiptSharing.Api.Controllers;
using ReceiptSharing.Api.Repositories;
using ReceiptSharing.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using ReceiptSharing.Test.TestUtils;
using ReceiptSharing.Test.CreateObject.Utils;

namespace ReceiptSharing.Test{

    public class SubscriptionReceiptControllerTest
    {
        private readonly ISubscriptionReceiptRepository _subscriptionRepository;
        private readonly ILogger<SubscriptionReceiptController> _logger;
        private readonly SubscriptionReceiptController _controller;

        public SubscriptionReceiptControllerTest()
        {
            _subscriptionRepository = Substitute.For<ISubscriptionReceiptRepository>();
            _logger = Substitute.For<ILogger<SubscriptionReceiptController>>();

            _controller = new SubscriptionReceiptController(_subscriptionRepository, _logger);

            var user = CreateRandomUser.CreateUser();
            var httpContext = new DefaultHttpContext();

            UserInHttpContext.SetUserInHttpContext(user, httpContext);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async void SubscribingToReceipt_WithCorrectData_ReturnsOk()
        {
            // Arrange
            var receiptId = 123;
            _subscriptionRepository.SubscribeToReceiptAsync(Arg.Any<SubscriptionReceipt>()).Returns(true);
            // Act
            var result = await _controller.SubscribeToReceiptAsync(receiptId);

            // Assert
             Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void SubscribingToReceipt_WithIncorrectForgeinKey_ReturnsNotFound()
        {
            // Arrange
            var receiptId = 123;
            _subscriptionRepository.SubscribeToReceiptAsync(Arg.Any<SubscriptionReceipt>()).Returns(false);
            // Act
            var result = await _controller.SubscribeToReceiptAsync(receiptId);

            // Assert
             Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async void UnSubscribingToReceipt_WithCorrectData_ReturnsOk()
        {
            // Arrange
            var receiptId = 123;
            _subscriptionRepository.UnsubscribeFromReceiptAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(true);

            // Act
            var result = await _controller.UnsubscribeFromReceiptAsync(receiptId);

            // Assert
             Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void UnSubscribingToReceipt_WithIncorrectForgeinKey_ReturnsNotFound()
        {
            // Arrange
            var receiptId = 123;
            _subscriptionRepository.UnsubscribeFromReceiptAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(false);

            // Act
            var result = await _controller.UnsubscribeFromReceiptAsync(receiptId);

            // Assert
             Assert.IsType<NotFoundObjectResult>(result);
        }

    }
}