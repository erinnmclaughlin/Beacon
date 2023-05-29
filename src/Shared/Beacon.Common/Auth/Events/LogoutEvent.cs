using MediatR;

namespace Beacon.Common.Auth.Events;
public sealed record LogoutEvent : INotification;
