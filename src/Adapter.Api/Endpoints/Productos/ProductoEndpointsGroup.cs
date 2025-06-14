using Adapter.Api.Configurations.EndpointsConfig;

namespace Adapter.Api.Endpoints.Productos
{
    public class ProductoEndpointsGroup : IEndpointGroup
    {
        const string productoRoute = "producto";
        public static string GroupName => productoRoute;
    }
}
