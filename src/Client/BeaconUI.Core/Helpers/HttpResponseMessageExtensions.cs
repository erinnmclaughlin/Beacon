using Beacon.Common.Validation;
using ErrorOr;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeaconUI.Core.Helpers;

public static class HttpResponseMessageExtensions
{
    // TODO: this doesn't super belong in here, but it's convenient, so leaving it here until it's an issue
    private static JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }

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
            var result = await response.Content.ReadFromJsonAsync<T>(JsonSerializerOptions, ct);
            return result is null ? Error.Unexpected() : result;
        }

        return await GetErrorResult<T>(response, ct);        
    }

    private static async Task<ErrorOr<T>> GetErrorResult<T>(this HttpResponseMessage response, CancellationToken ct = default)
    {
        if (response.StatusCode is HttpStatusCode.NotFound)
            return Error.NotFound();

        if (response.StatusCode is HttpStatusCode.UnprocessableEntity)
        {
            var validationProblem = await response.Content.ReadFromJsonAsync<BeaconValidationProblem>(JsonSerializerOptions, ct);
            var errors = validationProblem?.Errors.SelectMany(e => e.Value.Select(v => Error.Validation(e.Key, v))).ToList();
            return errors is null ? Error.Unexpected() : errors;
        }

        return Error.Failure();
    }
}