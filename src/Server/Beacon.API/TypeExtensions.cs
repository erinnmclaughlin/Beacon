using Beacon.API.Features;

namespace Beacon.API;

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
            i.GetGenericArguments().Length == 2 &&
            i.IsAssignableTo(typeof(IBeaconRequestHandler)));

        var genericArgumentTypes = interfaceType.GetGenericArguments();
        return (genericArgumentTypes[0], genericArgumentTypes[1]);
    }

}
