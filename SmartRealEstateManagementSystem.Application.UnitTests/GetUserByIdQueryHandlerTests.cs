using Application.DTO;
using Application.Use_Cases.Queries.UserQ;
using Application.Use_Cases.QueryHandlers.UserQH;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly IGenericEntityRepository<User> repository;
        private readonly IMapper mapper;
        private readonly GetUserByIdQueryHandler handler;

        public GetUserByIdQueryHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<User>>();
            mapper = Substitute.For<IMapper>();
            handler = new GetUserByIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetUserByIdQueryHandler_When_HandleIsCalled_Then_TheUserShouldBeReturned()
        {
            // Arrange
            var user = new User
            {
                Id = new Guid("d2aca8c8-ea05-4303-ad6f-83b41d71f97c"),
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@gmail.com",
                Password = "12345678"
            };
            var query = new GetUserByIdQuery { Id = user.Id };
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password
            };
            mapper.Map<UserDto>(user).Returns(userDto);
            repository.GetByIdAsync(user.Id).Returns(user);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.FirstName.Should().Be(user.FirstName);
            result.LastName.Should().Be(user.LastName);
        }

        [Fact]
        public async Task Given_GetUserByIdQueryHandler_When_HandleIsCalledWithInexistentId_Then_ShouldReturnNull()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = new Guid("d2aca8c8-ea05-4303-ad6f-83b41d71f97c") };
            repository.GetByIdAsync(query.Id).Returns((User?)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
