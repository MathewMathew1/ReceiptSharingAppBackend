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
using Microsoft.EntityFrameworkCore.Storage;

namespace ReceiptSharing.Test{

    public class ReceiptControllerTest
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly ILogger<ReceiptController> _logger;
        private readonly ISubscriptionUserRepository _subscriptionUserRepository;
        private readonly ReceiptController _controller;
        private readonly IMapper _mapper;
        private readonly ISubscriptionReceiptRepository _subscriptionRepository;
        private readonly User _user;
        private readonly IImageStorage _imageStorage;

        public ReceiptControllerTest()
        {
            _subscriptionRepository = Substitute.For<ISubscriptionReceiptRepository>();
            _subscriptionUserRepository = Substitute.For<ISubscriptionUserRepository>();
            _receiptRepository = Substitute.For<IReceiptRepository>();
            _logger = Substitute.For<ILogger<ReceiptController>>();
            _imageStorage = Substitute.For<IImageStorage>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = configuration.CreateMapper();

            _controller = new ReceiptController(_receiptRepository, _logger,  _imageStorage, _mapper, _subscriptionRepository, _subscriptionUserRepository);

            _user = CreateRandomUser.CreateUser();
            var httpContext = new DefaultHttpContext();

            UserInHttpContext.SetUserInHttpContext(_user, httpContext);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async void CreateReceipt_WithCorrectData_ReturnsOkAndCorrectReceipt()
        {
            // Arrange
            var receiptCommand = CreateRandomReceiptCommand.CreateReceipt();
            _imageStorage.PostImage(Arg.Any<IFormFile>()).Returns("imageLink");
            var expectedReceipt = new Receipt
                {
                    UserId = _user.Id,
                    Title = receiptCommand.Title,
                    Description = receiptCommand.Description,
                    Steps = receiptCommand.Steps,
                    ImageLinks = (await Task.WhenAll(receiptCommand.Images.Select(async image => await _imageStorage.PostImage(image)))).ToArray(),
                    VideoLink = receiptCommand.VideoLink,
                    MinCookDuration = receiptCommand.MinCookDuration,
                    MaxCookDuration = receiptCommand.MaxCookDuration,
                    Ingredients = receiptCommand.Ingredients
                };

            _receiptRepository.CreateReceiptAsync(Arg.Any<Receipt>()).ReturnsForAnyArgs(args => args.ArgAt<Receipt>(0) );
            // Act
            var actionResult = await _controller.CreateReceipt(receiptCommand);
            ObjectResult result = actionResult.Result as ObjectResult;
            ResponseCreateReceipt actualValue = result.Value! as ResponseCreateReceipt;
            var receipt = actualValue!.receipt;

            // Assert
            Assert.IsType<ActionResult<ResponseCreateReceipt>>(actionResult);
            Assert.True(CompareReceipts.AreReceiptFieldsMatching(expectedReceipt, receipt!));

        }

        [Fact]
        public async void GetReceipt_WithExistingReceipt_ReturnsOkAndReceiptDto()
        {
            // Arrange
            var receipt = CreateRandomReceipt.CreateReceipt();

            _receiptRepository.GetReceiptByIdAsync(Arg.Any<int>()).ReturnsForAnyArgs(receipt);
            // Act
            var actionResult = await _controller.GetReceiptById(receipt.Id);
            ObjectResult result = actionResult.Result as ObjectResult;
            ResponseGetReceipt actualValue = result.Value! as ResponseGetReceipt;
            var receiptDto = actualValue.receipt;
            // Assert
            Assert.True(CompareReceipts.AreReceiptFieldsMatching(receipt!, receiptDto ));
           
        }

        [Fact]
        public async void GetReceipt_WithNonExistingReceipt_ReturnsNotFound()
        {
            // Arrange
         

            _receiptRepository.GetReceiptByIdAsync(Arg.Any<int>()).ReturnsForAnyArgs((Receipt?)null);
            // Act
            var actionResult = await _controller.GetReceiptById(1);
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.Equal(404, result.StatusCode);
           
        }

        [Fact]
        public async void DeleteReceipt_WithExistingReceipt_ReturnsOk()
        {
            int receiptId = 1;
            // Arrange

            _receiptRepository.DeleteReceiptAsync(Arg.Any<int>(), Arg.Any<int>()).ReturnsForAnyArgs(true);
            // Act
            var actionResult = await _controller.DeleteReceipt(receiptId);
 
            // Assert
            Assert.IsType<OkObjectResult>(actionResult);
           
        }

        [Fact]
        public async void DeleteReceipt_WithNonExistingReceipt_ReturnsNotFound()
        {
            int receiptId = 1;
            // Arrange

            _receiptRepository.DeleteReceiptAsync(Arg.Any<int>(), Arg.Any<int>()).ReturnsForAnyArgs(false);
            // Act
            var actionResult = await _controller.DeleteReceipt(receiptId);
 
            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult);
           
        }

        [Fact]
        public async void UpdateReceipt_WithNonExistingReceipt_ReturnsNotFound()
        {
            int receiptId = 1;
            var updateReceipt = UpdateRandomReceiptCommand.CreateUpdateReceipt();
            // Arrange

            _receiptRepository.GetReceiptByIdAsync(Arg.Any<int>()).ReturnsForAnyArgs((Receipt?)null);
            // Act
            var actionResult = await _controller.UpdateReceipt(receiptId, updateReceipt);
 
            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult);
           
        }

        [Fact]
        public async void UpdateReceipt_WithExistingReceiptButNotAuthorized_ReturnsForbidden()
        {
            int receiptId = 1;
            var updateReceipt = UpdateRandomReceiptCommand.CreateUpdateReceipt();
            var receipt = CreateRandomReceipt.CreateReceipt();
            receipt.UserId = _user.Id+1;
            // Arrange

            _receiptRepository.GetReceiptByIdAsync(Arg.Any<int>()).ReturnsForAnyArgs(receipt);
            // Act
            var actionResult = await _controller.UpdateReceipt(receiptId, updateReceipt);
 
            // Assert
            Assert.IsType<ForbidResult>(actionResult);
           
        }

        [Fact]
        public async void UpdateReceipt_WithExistingReceiptAndAuthorized_ReturnsOkAndUpdatedReceipt()
        {
            // Arrange
            int receiptId = 1;
            var updateReceipt = UpdateRandomReceiptCommand.CreateUpdateReceipt();
            var receipt = CreateRandomReceipt.CreateReceipt();
            receipt.UserId = _user.Id;
            
            _receiptRepository.GetReceiptByIdAsync(Arg.Any<int>()).ReturnsForAnyArgs(receipt);
            // Act
            var actionResult = await _controller.UpdateReceipt(receiptId, updateReceipt);
 
            // Assert
            Assert.IsType<OkObjectResult>(actionResult);
           
        }

        [Fact]
        public async Task GetUserSubscribedReceipt_WithNoProblem_ReturnsReceipts()
        {
            // Arrange
            List<Receipt> receipts =  new () { CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt(),
            CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt()};
            _subscriptionRepository.GetSubscribedReceiptsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>()).Returns((receipts, count: receipts.Count));

            // Act
            var actionResult = await _controller.GetSubscribedReceipts();
            ObjectResult result = actionResult.Result as ObjectResult;
            ReceiptDtoListResponseWithCount actualValue = result.Value! as ReceiptDtoListResponseWithCount;
            var receiptDtos = actualValue.receipts;

            // Assert
            Assert.IsType<ActionResult<ReceiptDtoListResponseWithCount>>(actionResult);
            for (int i = 0; i < receipts.Count; i++)
            {
                // Implement your condition here
                bool conditionMet = CompareReceipts.AreReceiptFieldsMatching(receipts[i], receiptDtos[i]);
                
                Assert.True(conditionMet);
            }
        }

        [Fact]
        public async Task GetSubscribedToUserReceipt_WithNoProblem_ReturnsReceipts()
        {
            // Arrange
            List<Receipt> receipts =  new () { CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt(),
            CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt()};
            IEnumerable<SubscriptionUser> subscribedToIds = new List<SubscriptionUser> { CreateRandomSubscription.CreateSubscription(), CreateRandomSubscription.CreateSubscription(), 
                CreateRandomSubscription.CreateSubscription(), CreateRandomSubscription.CreateSubscription() };
            _subscriptionUserRepository.GetUserSubscriptionsAsync(Arg.Any<int>()).Returns(subscribedToIds);
            _receiptRepository.GetNewestSubscribedReceiptsAsync(Arg.Any<List<int>>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>()).Returns((receipts, count: receipts.Count));
            // Act
            var actionResult = await _controller.GetNewestUserSubscribedReceipts();
            ObjectResult result = actionResult.Result as ObjectResult;
            ReceiptDtoListResponseWithCount actualValue = result.Value! as ReceiptDtoListResponseWithCount;
            var receiptDtos = actualValue.receipts;

            // Assert
            Assert.IsType<ActionResult<ReceiptDtoListResponseWithCount>>(actionResult);
            for (int i = 0; i < receipts.Count; i++)
            {
                // Implement your condition here
                bool conditionMet = CompareReceipts.AreReceiptFieldsMatching(receipts[i], receiptDtos[i]);
                
                Assert.True(conditionMet);
            }
        }

        [Fact]
        public async Task GetReceiptSortedByRatings_WithNoProblem_ReturnsReceipts()
        {
            // Arrange
            List<Receipt> receipts =  new () { CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt(),
            CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt()};
 
            _receiptRepository.GetSubscribedReceiptsWithBayesianRatingAsync(Arg.Any<int>(), Arg.Any<int>()).Returns((receipts, false));
            // Act
            var actionResult = await _controller.GetSortedReceipts("bestRated");
            ObjectResult result = actionResult.Result as ObjectResult;
            ReceiptDtoListResponse actualValue = result.Value! as ReceiptDtoListResponse;
            var receiptDtos = actualValue.receipts;

            // Assert
            Assert.IsType<ActionResult<ReceiptDtoListResponse>>(actionResult);
            for (int i = 0; i < receipts.Count; i++)
            {
                // Implement your condition here
                bool conditionMet = CompareReceipts.AreReceiptFieldsMatching(receipts[i], receiptDtos[i]);
                
                Assert.True(conditionMet);
            }
        }

        [Fact]
        public async Task GetReceiptSortedByNewest_WithNoProblem_ReturnsReceipts()
        {
            // Arrange
            List<Receipt> receipts =  new () { CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt(),
            CreateRandomReceipt.CreateReceipt(), CreateRandomReceipt.CreateReceipt()};
 
            _receiptRepository.GetReceiptsSortedByNewSubscriptionsAsync(Arg.Any<int>(), Arg.Any<int>()).Returns((receipts, false));
            // Act
            var actionResult = await _controller.GetSortedReceipts("newest");
            ObjectResult result = actionResult.Result as ObjectResult;
            ReceiptDtoListResponse actualValue = result.Value! as ReceiptDtoListResponse;
            var receiptDtos = actualValue.receipts;

            // Assert
            Assert.IsType<ActionResult<ReceiptDtoListResponse>>(actionResult);
            for (int i = 0; i < receipts.Count; i++)
            {
                // Implement your condition here
                bool conditionMet = CompareReceipts.AreReceiptFieldsMatching(receipts[i], receiptDtos[i]);
                
                Assert.True(conditionMet);
            }
        }




    }
}