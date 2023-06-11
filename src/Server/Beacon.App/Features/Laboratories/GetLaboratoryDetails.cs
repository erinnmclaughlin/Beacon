using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Laboratories.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories;

public static class GetLaboratoryDetails
{
    public sealed record Query(Guid LaboratoryId) : IRequest<Response>;

    public sealed record Response(LaboratoryDto? Laboratory);

    public sealed record LaboratoryDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required List<MemberDto> Members { get; init; }
    }

    public sealed record MemberDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
        public required LaboratoryMembershipType MembershipType { get; init; }
    }

    public sealed class QueryHandler : IRequestHandler<Query, Response>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public QueryHandler(ICurrentUser currentUser, IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var lab = await _unitOfWork
                .QueryFor<Laboratory>()
                .Where(l => l.Id == request.LaboratoryId)
                .Select(l => new LaboratoryDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Members = l.Memberships.Select(m => new MemberDto
                    {
                        Id = m.Member.Id,
                        DisplayName = m.Member.DisplayName,
                        EmailAddress = m.Member.EmailAddress,
                        MembershipType = m.MembershipType
                    }).ToList()
                })
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

            if (lab is not null && !lab.Members.Any(m => m.Id == _currentUser.UserId))
                return new Response(null);

            return new Response(lab);
        }
    }
}
