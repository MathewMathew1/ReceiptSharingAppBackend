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

    public class RatingControllerTest
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ILogger<RatingController> _logger;
        private readonly IMapper _mapper;
        private readonly RatingController _controller;
        private readonly User _user;

        public RatingControllerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _ratingRepository = Substitute.For<IRatingRepository>();
            _logger = Substitute.For<ILogger<RatingController>>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = configuration.CreateMapper();
            _controller = new RatingController(_ratingRepository, _logger, _mapper);

            _user = CreateRandomUser.CreateUser();
            var httpContext = new DefaultHttpContext();

            UserInHttpContext.SetUserInHttpContext(_user, httpContext);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task RateReceipt_WithValidInput_ReturnsOkWithRating()
        {
            // Arrange
            var rating = CreateRating.CreateRate(2);
            var receiptId = 1;
            var ratingExpectedToReturn = new Rating { UserId = _user.Id, ReceiptId = receiptId, Value = rating.Rate };

            _ratingRepository.AddRatingAsync(Arg.Any<Rating>()).Returns(args => args.ArgAt<Rating>(0));

            // Act
            var actionResult = await _controller.RateReceipt(receiptId, rating);
            ObjectResult result = actionResult.Result as ObjectResult;
            ResponseCreateRatingMessage actualValue = result.Value! as ResponseCreateRatingMessage;
            var ratingCreated = actualValue!.rating;

            // Assert
            Assert.IsType<ActionResult<ResponseCreateRatingMessage>>(actionResult);
            ratingCreated.Should().BeEquivalentTo(
                ratingExpectedToReturn,
                options => options.ComparingByMembers<Rating>()
            );
        }

        [Fact]
        public async Task RateReceipt_WithInvalidReceiptId_ReturnsBadRequest()
        {
            // Arrange
            var rating = CreateRating.CreateRate(2);

            _ratingRepository.AddRatingAsync(Arg.Any<Rating>()).Returns((Rating?)null);

            // Act
            var actionResult = await _controller.RateReceipt(1, rating);
            ObjectResult result = actionResult.Result as ObjectResult;

            // Assert
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task UpdateRating_WithCorrectData_ReturnsOk()
        {
            // Arrange
            var receiptId = 123;
            var ratingDto = CreateRating.CreateRate(2);

            _ratingRepository.UpdateRatingAsync(Arg.Any<Rating>()).Returns(true);

            // Act
            var result = await _controller.UpdateRating(receiptId, ratingDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateRating__WithNoneExistingForgeinKeyForReceipt_ReturnsNotFound()
        {
            // Arrange
            var receiptId = 123;
            var ratingDto = CreateRating.CreateRate(2);

            _ratingRepository.UpdateRatingAsync(Arg.Any<Rating>()).Returns(false);

            // Act
            var result = await _controller.UpdateRating(receiptId, ratingDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

    }
}