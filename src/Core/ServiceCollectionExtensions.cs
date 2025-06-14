using Core.Contracts.UseCases.Productos;
using Core.UseCases.Productos;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core;
public static class ServiceCollectionExtensions
{
    public static void AddCore(this IServiceCollection services)
    {
        services.AddFluentValidation();
        services.AddUseCases();
    }

    private static void AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void AddUseCases(this IServiceCollection services)
    {
        services.AddProductoUseCases();
    }
    private static void AddProductoUseCases(this IServiceCollection services)
    {
        services.AddScoped<ICreateProducto, CreateProducto>()
                .AddScoped<IGetProductos, GetProductos>();
    }
}