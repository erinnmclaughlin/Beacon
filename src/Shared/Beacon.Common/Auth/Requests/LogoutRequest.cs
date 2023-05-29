using ErrorOr;

namespace Beacon.Common.Auth.Requests;

public sealed record LogoutRequest : IApiRequest<Success>;
