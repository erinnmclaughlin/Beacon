using Beacon.Common.Validation;
using ErrorOr;
using System.Net;
using System.Net.Http.Json;

namespace BeaconUI.Core;

public static class HttpResponseMessageExtensions
{
    public static async Task<ErrorOr<T>> ToErrorOrResult<T>(this HttpResponseMessage response, CancellationToken ct = default)
    {
        if (response.StatusCode is HttpStatusCode.NotFound)
            return Error.NotFound();

        if (response.StatusCode is HttpStatusCode.UnprocessableEntity)
        {
            var validationProblem = await response.Content.ReadFromJsonAsync<BeaconValidationProblem>(cancellationToken: ct);
            var errors = validationProblem?.Errors.SelectMany(e => e.Value.Select(v => Error.Validation(e.Key, v))).ToList();
            return errors is null ? Error.Unexpected() : errors;
        }

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
            return result is null ? Error.Unexpected() : result;
        }

        return Error.Failure();
    }
}