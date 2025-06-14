namespace Adapter.Api.Configurations.EndpointsConfig;
public interface IEndpoint<TGroup> where TGroup : IEndpointGroup
{
    RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app);
}