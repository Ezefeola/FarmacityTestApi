using Adapter.Api.Configurations.EndpointsConfig;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;
using Core.Contracts.UseCases.Productos;
using Microsoft.AspNetCore.Mvc;

namespace Adapter.Api.Endpoints.Productos;
public class SoftDeleteProductoEndpoint : IEndpoint<ProductoEndpointsGroup>
{
    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapDelete("/{productoId:int}", SoftDeleteProductoHandler)
                  .WithName("SoftDeleteProducto")
                  .Produces<SoftDeleteProductoResponseDto>(StatusCodes.Status200OK)
                  .ProducesProblem(StatusCodes.Status404NotFound)
                  .WithSummary("Soft Delete Producto")
                  .WithDescription("Soft Delete Producto");
    }

    private static async Task<Result<SoftDeleteProductoResponseDto>> SoftDeleteProductoHandler(
        [FromRoute] int productoId,
        [FromServices] ISoftDeleteProducto useCase,
        CancellationToken cancellationToken
    )
    {
        Result<SoftDeleteProductoResponseDto> response = await useCase.ExecuteAsync(productoId, cancellationToken);
        return response;
    }
}