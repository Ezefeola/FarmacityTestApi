using Adapter.Api.Configurations.EndpointsConfig;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;
using Core.Contracts.UseCases.Productos;
using Microsoft.AspNetCore.Mvc;

namespace Adapter.Api.Endpoints.Productos;

public class CreateProductoEndpoint : IEndpoint<ProductoEndpointsGroup>
{
    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost("/", CreateProductoHandler)
                  .WithName("CreateProducto")
                  .Produces<CreateProductoResponseDto>(StatusCodes.Status200OK)
                  .ProducesProblem(StatusCodes.Status400BadRequest)
                  .WithSummary("Create Producto")
                  .WithDescription("Create Producto");
    }

    private static async Task<Result<CreateProductoResponseDto>> CreateProductoHandler(
        [FromBody] CreateProductoRequestDto requestDto,
        [FromServices] ICreateProducto useCase,
        CancellationToken cancellationToken
    )
    {
        Result<CreateProductoResponseDto> response = await useCase.ExecuteAsync(requestDto, cancellationToken);
        return response;
    }
}
