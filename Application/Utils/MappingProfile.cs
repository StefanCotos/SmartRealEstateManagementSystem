using Application.DTO;
using Domain.Entities;
using AutoMapper;
using Application.Use_Cases.Commands.EstateC;
using Application.Use_Cases.Commands.UserC;
using Application.Use_Cases.Commands.FavoriteC;
using Application.Use_Cases.Commands.ImageC;
using Application.Use_Cases.Commands.ReportC;
using Application.Use_Cases.Commands.ReviewUserC;
using Application.Use_Cases.ActionsOnUser;

namespace Application.Utils
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Report, ReportDto>().ReverseMap();
            CreateMap<CreateReportCommand, Report>().ReverseMap();
            CreateMap<DeleteReportCommand, Report>().ReverseMap();

            CreateMap<Estate,EstateDto>().ReverseMap();
            CreateMap<CreateEstateCommand,Estate>().ReverseMap();
            CreateMap<UpdateEstateCommand, Estate>().ReverseMap();
            CreateMap<DeleteEstateCommand, Estate>().ReverseMap();

            CreateMap<Favorite, FavoriteDto>().ReverseMap();
            CreateMap<CreateFavoriteCommand, Favorite>().ReverseMap();
            CreateMap<DeleteFavoriteCommand, Favorite>().ReverseMap();

            CreateMap<Image, ImageDto>().ReverseMap();
            CreateMap<CreateImageCommand, Image>().ReverseMap();
            CreateMap<UpdateImageCommand, Image>().ReverseMap();
            CreateMap<DeleteImageCommand, Image>().ReverseMap();

            CreateMap<Report, ReportDto>().ReverseMap();
            CreateMap<CreateReportCommand, Report>().ReverseMap();

            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<CreateReviewUserCommand, Review>().ReverseMap();

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserCommand, User>().ReverseMap();
            CreateMap<UpdateUserCommand, User>().ReverseMap();
            CreateMap<DeleteUserCommand, User>().ReverseMap();

            CreateMap<EditUserCommand, User>().ReverseMap();
            CreateMap<RemoveUserCommand, User>().ReverseMap();
            CreateMap<CheckPasswordCommand, User>().ReverseMap();
        }
    }
}
