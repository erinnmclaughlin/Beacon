using Beacon.API.App.Services;
using Beacon.API.Domain.Entities;
using Beacon.API.Domain.Exceptions;
using Beacon.Common.Laboratories.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.App.Features.Laboratories;

public static class GetUserMemberships
{
    public sealed record Query(Guid MemberId) : IRequest<Response>;

    public sealed record Response(List<LaboratoryMembershipDto> Memberships);

    public sealed record LaboratoryMembershipDto
    {
        public required LaboratoryDto Laboratory { get; init; }
        public required UserDto Member { get; init; }
        public required LaboratoryMembershipType MembershipType { get; init; }
    }

    public sealed record LaboratoryDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
    }

    public sealed record UserDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
    }

    internal sealed class QueryHandler : IRequestHandler<Query, Response>
    {
        private readonly IUnitOfWork _unitOfWork;

        public QueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var memberships = await _unitOfWork
                .GetRepository<LaboratoryMembership>()
                .AsQueryable()
                .Where(m => m.MemberId == request.MemberId)
                .Select(m => new LaboratoryMembershipDto
                {
                    Laboratory = new LaboratoryDto
                    {
                        Id = m.Laboratory.Id,
                        Name = m.Laboratory.Name
                    },
                    Member = new UserDto
                    {
                        Id = m.Member.Id,
                        DisplayName = m.Member.DisplayName,
                        EmailAddress = m.Member.EmailAddress
                    },
                    MembershipType = m.MembershipType
                })
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            if (!memberships.Any())
            {
                await VerifyThatUserExists(request.MemberId, cancellationToken);
            }

            return new Response(memberships);
        }

        private async Task VerifyThatUserExists(Guid userId, CancellationToken ct)
        {
            var userExists = await _unitOfWork.GetRepository<User>().AsQueryable().AnyAsync(u => u.Id == userId, ct);

            if (!userExists)
                throw new UserNotFoundException(userId);
        }
    }
}
