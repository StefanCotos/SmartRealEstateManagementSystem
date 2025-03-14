using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.UserQ
{
    public class GetUserByIdQuery: IRequest<UserDto>
    {
        public Guid Id { get; set; }
    }
}
