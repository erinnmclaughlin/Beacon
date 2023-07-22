using ErrorOr;

namespace Beacon.API.Features;

public static class BeaconError
{
    public static Error Forbid(string description = "The current user is not authorized to perform that request.") => Error.Custom(403, "Forbid", description);
    public static Error Unauthorized(string description = "") => Error.Custom(401, "Unauthorized", description);
    public static Error Validation(string propertyName, string errorMessage) => Error.Validation(propertyName, errorMessage);
}
