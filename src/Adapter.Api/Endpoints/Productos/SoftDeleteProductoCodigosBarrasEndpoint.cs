using Adapter.Api.Configurations.EndpointsConfig;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;
using Core.Contracts.UseCases.Productos;
using Microsoft.AspNetCore.Mvc;

namespace Adapter.Api.Endpoints.Productos;
public class SoftDeleteProductoCodigosBarrasEndpoint : IEndpoint<ProductoEndpointsGroup>
{
    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapDelete("/{productoId:int}/codigos-barras", SoftDeleteProductoCodigoBarraHandler)
                  .WithName("SoftDeleteProductosCodigosBarras")
                  .Produces<SoftDeleteProductoCodigoDeBarraResponseDto>(StatusCodes.Status200OK)
                  .ProducesProblem(StatusCodes.Status400BadRequest)
                  .ProducesProblem(StatusCodes.Status404NotFound)
                  .WithSummary("Soft Delete Producto Codigos Barras")
                  .WithDescription("Soft Delete Producto Codigos Barras");
    }

    public static async Task<Result<SoftDeleteProductoCodigoDeBarraResponseDto>> SoftDeleteProductoCodigoBarraHandler(
        [FromRoute] int productoId,
        [AsParameters] SoftDeleteProductoCodigoDeBarraRequestDto parametersRequestDto,
        [FromServices] ISoftDeleteProductoCodigoBarra useCase,
        CancellationToken cancellationToken
    )
    {
        Result<SoftDeleteProductoCodigoDeBarraResponseDto> response = await useCase.ExecuteAsync(
            productoId,
            parametersRequestDto,
            cancellationToken
        );
        return response;
    }
}