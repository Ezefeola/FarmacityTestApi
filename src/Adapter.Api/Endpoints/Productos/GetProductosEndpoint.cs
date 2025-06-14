using Adapter.Api.Configurations.EndpointsConfig;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;
using Core.Contracts.UseCases.Productos;
using Microsoft.AspNetCore.Mvc;

namespace Adapter.Api.Endpoints.Productos
{
    public class GetProductosEndpoint : IEndpoint<ProductoEndpointsGroup>
    {
        public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
        {
            return app.MapGet("/", GetProductosHandler)
                      .WithName("GetProductos")
                      .Produces<GetProductosResponseDto>(StatusCodes.Status200OK)
                      .ProducesProblem(StatusCodes.Status404NotFound)
                      .WithSummary("Get Productos")
                      .WithDescription("Get Producto");
        }

        private static async Task<Result<GetProductosResponseDto>> GetProductosHandler(
            [AsParameters] GetProductosRequestDto parametersRequestDto,
            [FromServices] IGetProductos useCase,
            CancellationToken cancellationToken
        )
        {
            Result<GetProductosResponseDto> response = await useCase.ExecuteAsync(parametersRequestDto, cancellationToken);
            return response;
        }
    }
}
