using MediatR;

namespace Beacon.Common.Auth.Events;
public sealed record LoginEvent(UserDto LoggedInUser) : INotification;
