using System.Text.Json.Serialization;

namespace Beacon.Common.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LaboratoryMembershipType
{
    Member = 0,
    Analyst = 1,
    Manager = 2,
    Admin = 3
}
