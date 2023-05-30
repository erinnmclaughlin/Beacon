using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Helpers;
using ErrorOr;

namespace BeaconUI.Core.Laboratories.RequestHandlers;

internal class GetLaboratoryMembershipsByLabIdRequestHandler : IApiRequestHandler<GetLaboratoryMembershipsByLabIdRequest, List<LaboratoryMembershipDto>>
{
    private readonly HttpClient _httpClient;

    public GetLaboratoryMembershipsByLabIdRequestHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<List<LaboratoryMembershipDto>>> Handle(GetLaboratoryMembershipsByLabIdRequest request, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"api/laboratories/{request.LaboratoryId}/memberships", ct);
        return await response.ToErrorOrResult<List<LaboratoryMembershipDto>>(ct);
    }
}
