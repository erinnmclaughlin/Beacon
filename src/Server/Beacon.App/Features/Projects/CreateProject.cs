using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.App.ValueObjects;
using Beacon.Common.Validation.Rules;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Projects;

public static class CreateProject
{
    public sealed record Command(Guid Id, string CustomerCode, string CustomerName) : IRequest;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CustomerCode).IsValidCustomerCode();
            RuleFor(x => x.CustomerName).IsValidCustomerName();
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

        public async Task Handle(Command request, CancellationToken ct)
        {
            _unitOfWork.GetRepository<Project>().Add(new Project
            {
                Id = request.Id,
                ProjectCode = await GenerateProjectCode(request.CustomerCode, ct),
                CustomerName = request.CustomerName,
                CreatedById = _sessionManager.UserId,
                LaboratoryId = _sessionManager.LabId
            });

            await _unitOfWork.SaveChangesAsync(ct);
        }

        private async Task<ProjectCode> GenerateProjectCode(string customerCode, CancellationToken ct)
        {
            var lastSuffix = await _unitOfWork
                .QueryFor<Project>()
                .Where(p => p.ProjectCode.CustomerCode == customerCode)
                .OrderBy(p => p.ProjectCode.Suffix)
                .Select(p => p.ProjectCode.Suffix)
                .LastOrDefaultAsync(ct);

            return new ProjectCode(customerCode, lastSuffix + 1);
        }
    }
}
