using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.UserQ
{
    public class GetUsersQuery : IRequest<List<UserDto>>
    {

    }
}
