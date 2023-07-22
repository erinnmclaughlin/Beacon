using System.Text.Json;

namespace Beacon.Common.Requests;

public abstract class BeaconRequest<T> where T : BeaconRequest<T>
{
    public static bool TryParse(string value, out T result)
    {
        var data = JsonSerializer.Deserialize<T>(value);
        result = data!;
        return data != null;
    }
}
