using Adapter.Api.Filters;
using System.Reflection;

namespace Adapter.Api.Configurations.EndpointsConfig;
public static class EndpointExtensions
{
    public static void MapEndpoints(this IEndpointRouteBuilder builder, string? basePrefix)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var endpointTypes = assembly.GetTypes()
            .Where(t =>
                t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEndpoint<>)) &&
                !t.IsAbstract &&
                t.GetConstructor(Type.EmptyTypes) != null)
            .ToList();

        var methodName = typeof(IEndpoint<>).GetMethods()
            .First(m => m.Name == "MapEndpoint").Name;

        foreach (var endpointType in endpointTypes)
        {
            var interfaceType = endpointType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEndpoint<>));

            var groupType = interfaceType.GetGenericArguments()[0];

            var groupNameProp = groupType.GetProperty("GroupName", BindingFlags.Public | BindingFlags.Static);
            var groupName = (string?)groupNameProp?.GetValue(null) ?? groupType.Name.ToLower();

            var prefix = $"{basePrefix}/{groupName}".TrimEnd('/');
            var tag = groupName;

            var routeGroup = builder.MapGroup(prefix)
                .AddEndpointFilter<ResultHttpCodeFilter>()
                .WithTags(tag)
                .WithOpenApi();

            var endpointInstance = Activator.CreateInstance(endpointType);

            var method = interfaceType.GetMethod(methodName);
            _ = method?.Invoke(endpointInstance, new object[] { routeGroup });
        }
    }
}