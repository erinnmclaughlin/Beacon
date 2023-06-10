using Beacon.API.App.Services;
using Beacon.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.App.Features.Users;

public static class GetUserById
{
    public sealed record Query(Guid UserId) : IRequest<Response>;
    public sealed record Response(UserDto? User);

    public sealed record UserDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
    }

    public sealed class QueryHandler : IRequestHandler<Query, Response>
    {
        private readonly IUnitOfWork _unitOfWork;

        public QueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.GetRepository<User>()
                .AsQueryable()
                .Where(u => u.Id == request.UserId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    EmailAddress = u.EmailAddress
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return new Response(user);
        }
    }
}
