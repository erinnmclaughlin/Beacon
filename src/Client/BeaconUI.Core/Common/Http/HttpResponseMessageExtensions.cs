using Beacon.Common;
using Beacon.Common.Validation;
using ErrorOr;
using System.Net;
using System.Net.Http.Json;

namespace BeaconUI.Core.Common.Http;

public static class HttpResponseMessageExtensions
{
    public static async Task<ErrorOr<Success>> ToErrorOrResult(this HttpResponseMessage response, CancellationToken ct = default)
    {
        if (response.IsSuccessStatusCode)
            return Result.Success;

        return await response.ToErrorOrResult<Success>(ct);
    }

    public static async Task<ErrorOr<T>> ToErrorOrResult<T>(this HttpResponseMessage response, CancellationToken ct = default)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<T>(JsonDefaults.JsonSerializerOptions, ct);
            return result is null ? Error.Unexpected() : result;
        }

        return await response.GetErrorResult<T>(ct);
    }

    private static async Task<ErrorOr<T>> GetErrorResult<T>(this HttpResponseMessage response, CancellationToken ct = default)
    {
        if (response.StatusCode is HttpStatusCode.NotFound)
            return Error.NotFound();

        if (response.StatusCode is HttpStatusCode.UnprocessableEntity)
        {
            var validationProblem = await response.Content.ReadFromJsonAsync<BeaconValidationProblem>(JsonDefaults.JsonSerializerOptions, ct);
            var errors = validationProblem?.Errors.SelectMany(e => e.Value.Select(v => Error.Validation(e.Key, v))).ToList();
            return errors is null ? Error.Unexpected() : errors;
        }

        return Error.Failure();
    }
}