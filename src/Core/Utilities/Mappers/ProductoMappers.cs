using Core.Contracts.DTOs.CodigosBarras.Response;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Domain.Entities;
using Core.Utilities.QueryOptions;

namespace Core.Utilities.Mappers;
public static class ProductoMappers
{
    public static Producto ToEntity(this CreateProductoRequestDto requestDto, IEnumerable<string> codigosBarras)
    {
        return new Producto
        {
            Nombre = requestDto.Nombre,
            Precio = requestDto.Precio,
            CantidadEnStock = requestDto.CantidadEnStock,
            Activo = true,
            FechaAlta = DateTime.UtcNow,
            CodigosBarras = codigosBarras.Select(codigoBarra => new CodigoBarra
            {
                Codigo = codigoBarra,
                Activo = true,
                FechaAlta = DateTime.UtcNow
            }).ToList()
        };
    }

    public static CreateProductoResponseDto ToCreateProductoResponseDto(this Producto producto)
    {
        return new CreateProductoResponseDto
        {
            ProductoId = producto.Id,   
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            CantidadEnStock = producto.CantidadEnStock,
            Activo = producto.Activo,
            FechaAlta = producto.FechaAlta,
            CodigosBarras = producto.CodigosBarras.Select(codigosBarras => new CodigoBarraResponseDto
            {
                CodigoBarraId = codigosBarras.Id,  
                Codigo = codigosBarras.Codigo,
                Activo = codigosBarras.Activo,
                FechaAlta = codigosBarras.FechaAlta
            }).ToList()
        };
    }

    public static UpdateProductoResponseDto ToUpdateProductoResponseDto(this Producto producto)
    {
        return new UpdateProductoResponseDto
        {
            ProductoId = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            CantidadEnStock = producto.CantidadEnStock,
            Activo = producto.Activo,
            FechaAlta = producto.FechaAlta,
            FechaModificacion = producto.FechaModificacion,
            CodigosBarras = producto.CodigosBarras.Select(codigoBarra => new CodigoBarraResponseDto
            {
                CodigoBarraId = codigoBarra.Id,
                Codigo = codigoBarra.Codigo,
                Activo = codigoBarra.Activo,
                FechaAlta = codigoBarra.FechaAlta,
                FechaModificacion = codigoBarra.FechaModificacion
            }).ToList()
        };
    }

    public static GetProductosResponseDto ToGetProductosResponseDtos(
        this IEnumerable<Producto> productos,
        GetProductosRequestDto parametersRequestDto,
        int totalRecordsCount
    )
    {
        return new GetProductosResponseDto
        {
            PageIndex = parametersRequestDto.GetPage(),
            PageSize = parametersRequestDto.GetPageSize(),
            TotalPages = parametersRequestDto.GetTotalPages(totalRecordsCount),
            TotalRecords = totalRecordsCount,
            Items = productos.Select(producto => new ProductosResponseDto
            {
                ProductoId = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Activo = producto.Activo,
                CantidadEnStock = producto.CantidadEnStock,
                FechaAlta = producto.FechaAlta,
                FechaModificacion = producto.FechaModificacion,
                CodigosBarras = producto.CodigosBarras.Select(codigoBarra => new CodigoBarraResponseDto
                {
                    CodigoBarraId = codigoBarra.Id,
                    Codigo = codigoBarra.Codigo,
                    Activo = codigoBarra.Activo,
                    FechaAlta = codigoBarra.FechaAlta,
                    FechaModificacion = codigoBarra.FechaModificacion
                }).ToList()
            }).ToList(),
        };
    }

    public static GetProductoByIdResponseDto ToGetProductoByIdResponseDto(this Producto producto)
    {
        return new GetProductoByIdResponseDto()
        {
            ProductoId = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            CantidadEnStock = producto.CantidadEnStock,
            FechaAlta = producto.FechaAlta,
            FechaModificacion = producto.FechaModificacion,
            Activo = producto.Activo,
            CodigosBarras = producto.CodigosBarras.Select(codigoBarra => new CodigoBarraResponseDto
            {
                CodigoBarraId = codigoBarra.Id,
                Codigo = codigoBarra.Codigo,
                FechaAlta = codigoBarra.FechaAlta,
                FechaModificacion = codigoBarra.FechaModificacion,
                Activo = codigoBarra.Activo
            }).ToList()
        };
    }

    public static SoftDeleteProductoResponseDto ToSoftDeleteProductoResponseDto(this Producto producto)
    {
        return new SoftDeleteProductoResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Activo = producto.Activo,
            FechaAlta = producto.FechaAlta,
            FechaModificacion = producto.FechaModificacion,
            CodigosBarras = producto.CodigosBarras.Select(codigoBarra => new CodigoBarraResponseDto
            {
                CodigoBarraId = codigoBarra.Id,
                Codigo = codigoBarra.Codigo,
                Activo = codigoBarra.Activo,
                FechaAlta = codigoBarra.FechaAlta,
                FechaModificacion = codigoBarra.FechaModificacion,
            }).ToList()
        };
    }

    public static SoftDeleteProductoCodigoDeBarraResponseDto ToSoftDeleteProductoCodigoDeBarraResponseDto(this Producto producto)
    {
        return new SoftDeleteProductoCodigoDeBarraResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Activo = producto.Activo,
            FechaAlta = producto.FechaAlta,
            FechaModificacion = producto.FechaModificacion,
            CodigosBarras = producto.CodigosBarras.Where(x => x.Activo == false)
                                                    .Select(codigoBarra => new CodigoBarraResponseDto
            {
                CodigoBarraId = codigoBarra.Id,
                Codigo = codigoBarra.Codigo,
                Activo = codigoBarra.Activo,
                FechaAlta = codigoBarra.FechaAlta,
                FechaModificacion = codigoBarra.FechaModificacion,
            }).ToList()
        };
    }
}