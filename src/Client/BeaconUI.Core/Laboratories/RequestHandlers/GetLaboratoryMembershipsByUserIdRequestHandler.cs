using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Helpers;
using ErrorOr;

namespace BeaconUI.Core.Laboratories.RequestHandlers;

internal class GetLaboratoryMembershipsByUserIdRequestHandler : IApiRequestHandler<GetLaboratoryMembershipsByUserIdRequest, List<LaboratoryMembershipDto>>
{
    private readonly HttpClient _httpClient;

    public GetLaboratoryMembershipsByUserIdRequestHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<List<LaboratoryMembershipDto>>> Handle(GetLaboratoryMembershipsByUserIdRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"api/users/{request.UserId}/memberships", cancellationToken);
        return await response.ToErrorOrResult<List<LaboratoryMembershipDto>>(cancellationToken);
    }
}
