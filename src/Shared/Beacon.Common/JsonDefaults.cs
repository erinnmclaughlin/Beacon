using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beacon.Common;

public static class JsonDefaults
{
    public static JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}