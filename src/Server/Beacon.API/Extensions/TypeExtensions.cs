using Beacon.API.Features;
using ErrorOr;

namespace Beacon.API.Extensions;

public static class TypeExtensions
{
    public static bool IsBeaconRequestHandler(this Type t) =>
        t.IsClass &&
        t.GetInterfaces().Any(i =>
            i.IsGenericType &&
            i.IsAssignableTo(typeof(IBeaconRequestHandler)));

    public static (Type RequestType, Type ResponseType) GetRequestAndResponseTypes(this Type t)
    {
        if (!t.IsBeaconRequestHandler())
            throw new InvalidOperationException();

        var interfaceType = t.GetInterfaces().First(i =>
            i.IsGenericType &&
            i.IsAssignableTo(typeof(IBeaconRequestHandler)));

        var genericArgumentTypes = interfaceType.GetGenericArguments();
        var requestType = genericArgumentTypes[0];
        var responseType = genericArgumentTypes.Length > 1 ? genericArgumentTypes[1] : typeof(Success);
        
        return (requestType, responseType);
    }
}
