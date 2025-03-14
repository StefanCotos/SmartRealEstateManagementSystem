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
    public class GetUsersQueryHandlerTests
    {
        private readonly IGenericEntityRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly GetUsersQueryHandler _handler;

        public GetUsersQueryHandlerTests()
        {
            _userRepository = Substitute.For<IGenericEntityRepository<User>>();
            _mapper = Substitute.For<IMapper>();
            _handler = new GetUsersQueryHandler(_userRepository, _mapper);
        }

        [Fact]
        public async Task Given_GetUsersQueryHandler_When_HandleIsCalled_Then_AListOfUsersShouldBeReturned()
        {
            // Arrange
            List<User> users = GenerateUsers();
            _userRepository.GetAllAsync().Returns(users);
            var query = new GetUsersQuery();
            GenerateUsersDto(users);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            Assert.Equal(2, result.Count);
            Assert.Equal(users[0].Id, result[0].Id);
            Assert.Equal(users[1].Id, result[1].Id);
        }

        private void GenerateUsersDto(List<User> users)
        {
            _mapper.Map<List<UserDto>>(users).Returns(new List<UserDto>
            {
                new UserDto
                {
                    Id = users[0].Id,
                    FirstName = users[0].FirstName,
                    LastName = users[0].LastName,
                    UserName = users[0].UserName ?? string.Empty,
                    Email = users[0].Email,
                    Password = users[0].Password
                },
                new UserDto
                {
                    Id = users[1].Id,
                    FirstName = users[1].FirstName,
                    LastName = users[1].LastName,
                    UserName = users[1].UserName ?? string.Empty,
                    Email = users[1].Email,
                    Password = users[1].Password
                }
            });
        }

        static private List<User> GenerateUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "johndoe",
                    Email = "john@gmail.com",
                    Password = "password"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Jane",
                    LastName = "Smith",
                    UserName = "janesmith",
                    Email = "jane@gmail.com",
                    Password = "password"
                }
            };
        }
    }
}
