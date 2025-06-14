using Adapter.Api.Configurations.EndpointsConfig;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;
using Core.Contracts.UseCases.Productos;
using Microsoft.AspNetCore.Mvc;

namespace Adapter.Api.Endpoints.Productos
{
    public class UpdateProductoEndpoint : IEndpoint<ProductoEndpointsGroup>
    {
        public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
        {
            return app.MapPatch("/{productoId:int}", UpdateProductoHandler)
                      .WithName("UpdateProducto")
                      .Produces<UpdateProductoResponseDto>(StatusCodes.Status200OK)
                      .ProducesProblem(StatusCodes.Status404NotFound)
                      .ProducesProblem(StatusCodes.Status400BadRequest)
                      .WithSummary("Update Producto")
                      .WithDescription("Update Producto"); ;
        }

        private static async Task<Result<UpdateProductoResponseDto>> UpdateProductoHandler(
            [FromRoute] int productoId,
            [FromBody] UpdateProductoRequestDto requestDto,
            [FromServices] IUpdateProducto useCase,
            CancellationToken cancellationToken
        )
        {
            Result<UpdateProductoResponseDto> response = await useCase.ExecuteAsync(
                productoId,
                requestDto, 
                cancellationToken
            );
            return response;
        }
    }
}