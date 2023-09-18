using AutoMapper;
using ReceiptSharing.Api.Models;

namespace ReceiptSharing.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Receipt, ReceiptDto>()
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Ratings!.Any() ? src.Ratings!.Average(r => r.Value) : 0))
                .ForMember(dest => dest.NumberOfRatings, opt => opt.MapFrom(src => src.Ratings!.Count))
                .ForMember(dest => dest.NumberOfSubscriptions, opt => opt.MapFrom(src => src.SubscriptionsReceipt!.Count))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom<UserRatingOnReviewResolver>());
            CreateMap<Review, ReviewInReceipt>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<SubscriptionReceipt, SubscriptionReceiptDto>();
            CreateMap<SubscriptionUser, SubscriptionUserDto>();
            CreateMap<Review, ReviewDto>();
            CreateMap<User, UserDto>();
            CreateMap<Review, ReviewInReceipt>();
            CreateMap<User, OtherUserDto>();
            CreateMap<Rating, RatingDto>();
            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.NumberOfReviews, opt => opt.MapFrom(src => src.Reviews!.Count))
                .ForMember(dest => dest.NumberOfRatings, opt => opt.MapFrom(src => src.Ratings!.Count))
                .ForMember(dest => dest.NumberOfSubscriptions, opt => opt.MapFrom(src => src.SubscribedTo!.Count))
                .ForMember(dest => dest.NumberOfReceipts, opt => opt.MapFrom(src => src.Receipts!.Count));
            // Add more mappings as needed
        }
    }
}