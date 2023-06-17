using Beacon.App.Services;
using Beacon.Common.Validation.Rules;
using FluentValidation;
using MediatR;

namespace Beacon.App.Features.Projects;

public static class CreateProject
{
    public sealed record Command(Guid ProjectId, string CompanyName, string CompanyCode) : IRequest;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyCode).IsValidCompanyCode();
            RuleFor(x => x.CompanyName).IsValidCompanyName();
        }
    }

    internal sealed class Handler : IRequestHandler<Command>
    {
        private readonly ISessionManager _sessionManager;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(ISessionManager sessionManger, IUnitOfWork unitOfWork)
        {
            _sessionManager = sessionManger;
            _unitOfWork = unitOfWork;
        }

        public Task Handle(Command request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
