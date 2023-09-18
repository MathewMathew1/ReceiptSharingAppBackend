using ReceiptSharing.Api.Controllers;
using ReceiptSharing.Api.Repositories;
using ReceiptSharing.Api.Models;
using ReceiptSharing.Test.CreateUser.TestUtils;
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

    public class ReviewControllerTest
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ILogger<ReviewController> _logger;
        private readonly IMapper _mapper;
        private readonly User _user;
        private readonly ReviewController _controller;

        public ReviewControllerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _reviewRepository = Substitute.For<IReviewRepository>();
            _logger = Substitute.For<ILogger<ReviewController>>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = configuration.CreateMapper();
            _controller = new ReviewController(_reviewRepository, _logger, _mapper);

            _user = CreateRandomUser.CreateUser();
            var httpContext = new DefaultHttpContext();

            UserInHttpContext.SetUserInHttpContext(_user, httpContext);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task CreateReview_WithValidData_ReturnsOkWithReviewDto()
        {
            // Arrange
            var reviewCommand  = CreateRandomReviewCommand.CreateReview();
            var receiptId = 1;
            var reviewExpectedToReturn = new Review { UserId = _user.Id, ReceiptId = receiptId, ReviewText = reviewCommand.ReviewText };

            _reviewRepository.CreateReviewAsync(Arg.Any<Review>())
                .ReturnsForAnyArgs(args => args.ArgAt<Review>(0) );

            // Act
            var actionResult = await _controller.CreateReviewAsync(receiptId, reviewCommand);
            ObjectResult result = actionResult.Result as ObjectResult;
            ResponseCreateReviewMessage actualValue = result.Value! as ResponseCreateReviewMessage;
            var review = actualValue!.review;

            // Assert
            Assert.IsType<ActionResult<ResponseCreateReviewMessage>>(actionResult);
            review.Should().BeEquivalentTo(
                reviewExpectedToReturn,
                options => options.ComparingByMembers<Review>()
            );

        }

        [Fact]
        public async Task CreateReview_WithInvalidForgeinKey_ReturnsNotFound()
        {
            // Arrange
            var reviewCommand  = CreateRandomReviewCommand.CreateReview();
            var receiptId = 1;

            _reviewRepository.CreateReviewAsync(Arg.Any<Review>()).Returns((Review)null);

            // Act
            var actionResult = await _controller.CreateReviewAsync(receiptId, reviewCommand);
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.Equal(404, result.StatusCode);

        }

        [Fact]
        public async Task DeleteReview_WithInvalidForgeinKey_ReturnsNotFound()
        {
            // Arrange
            var receiptId = 1;

            _reviewRepository.DeleteReviewAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(false);

            // Act
            var actionResult = await _controller.DeleteReviewAsync(receiptId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult);

        }

        [Fact]
        public async Task DeleteReview_WithValidForgeinKey_ReturnsOk()
        {
            // Arrange
            var receiptId = 1;

            _reviewRepository.DeleteReviewAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(true);

            // Act
            var actionResult = await _controller.DeleteReviewAsync(receiptId);

            // Assert
            Assert.IsType<OkObjectResult>(actionResult);

        }

        [Fact]
        public async Task UpdateReview_WithValidForgeinKey_ReturnsOk()
        {
            // Arrange
            var receiptId = 1;
            var reviewCommand  = CreateRandomReviewCommand.UpdateReview();
            _reviewRepository.UpdateReviewAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<UpdateReviewCommand>()).Returns(true);

            // Act
            var actionResult = await _controller.UpdateReviewAsync(receiptId, reviewCommand);

            // Assert
            Assert.IsType<OkObjectResult>(actionResult);

        }

        [Fact]
        public async Task UpdateReview_WithInValidForgeinKey_ReturnsNotFound()
        {
            // Arrange
            var receiptId = 1;
            var reviewCommand  = CreateRandomReviewCommand.UpdateReview();
            _reviewRepository.UpdateReviewAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<UpdateReviewCommand>()).Returns(false);

            // Act
            var actionResult = await _controller.UpdateReviewAsync(receiptId, reviewCommand);

            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult);

        }

        

        

    }
}