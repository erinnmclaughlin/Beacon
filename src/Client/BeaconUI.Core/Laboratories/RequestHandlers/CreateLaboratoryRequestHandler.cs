using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Events;
using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Helpers;
using ErrorOr;
using MediatR;
using System.Net.Http.Json;

namespace BeaconUI.Core.Laboratories.RequestHandlers;

public class CreateLaboratoryRequestHandler : IApiRequestHandler<CreateLaboratoryRequest, LaboratoryDto>
{
    private readonly HttpClient _httpClient;
    private readonly IPublisher _publisher;

    public CreateLaboratoryRequestHandler(HttpClient httpClient, IPublisher publisher)
    {
        _httpClient = httpClient;
        _publisher = publisher;
    }

    public async Task<ErrorOr<LaboratoryDto>> Handle(CreateLaboratoryRequest request, CancellationToken ct)
    {
        var response = await _httpClient.PostAsJsonAsync("api/laboratories", request, ct);
        var result = await response.ToErrorOrResult<LaboratoryDto>(ct);

        if (!result.IsError)
            await _publisher.Publish(new LaboratoryCreatedEvent(result.Value), ct);

        return result;
    }
}
