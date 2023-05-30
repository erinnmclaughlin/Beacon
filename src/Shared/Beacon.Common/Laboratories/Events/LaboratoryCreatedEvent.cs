using MediatR;

namespace Beacon.Common.Laboratories.Events;

public sealed record LaboratoryCreatedEvent(LaboratoryDto Laboratory) : INotification;
