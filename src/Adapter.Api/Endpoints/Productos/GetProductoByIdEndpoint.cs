using Adapter.Api.Configurations.EndpointsConfig;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;
using Core.Contracts.UseCases.Productos;
using Microsoft.AspNetCore.Mvc;

namespace Adapter.Api.Endpoints.Productos;
public class GetProductoByIdEndpoint : IEndpoint<ProductoEndpointsGroup>
{
    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/{productoId:int}", GetProductoByIdHandler)
                  .WithName("GetProductoById")
                  .Produces<GetProductoByIdResponseDto>(StatusCodes.Status200OK)
                  .ProducesProblem(StatusCodes.Status404NotFound)
                  .WithSummary("Get Producto By Id")
                  .WithDescription("Get Producto By Id");
    }

    private static async Task<Result<GetProductoByIdResponseDto>> GetProductoByIdHandler(
        [FromRoute] int productoId,
        [FromServices] IGetProductoById useCase,
        CancellationToken cancellationToken
    )
    {
        Result<GetProductoByIdResponseDto> response = await useCase.ExecuteAsync(
            productoId,
            cancellationToken
        );
        return response;
    }
}