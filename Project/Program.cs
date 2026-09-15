using Microsoft.EntityFrameworkCore;
using Project.Properties.Dto;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // I read the connection string configured in User Secrets and assigned it to a variable
            var connection = builder.Configuration.GetConnectionString("DefaultConnection");

            // Configures AppDbContext to use SQL Server with the retrieved connection string and registers it in DI
            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(connection));
            
            builder.Services.AddScoped<IProductService, ProductService>();

            var app = builder.Build();

            // Create product
            app.MapPost("/products", async (CreateProductRequest request,IProductService service,CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrEmpty(request.Name) || request.Price < 0)
                {
                    return Results.BadRequest("Invalid product data.");
                }

                var product = await service.CreateProductAsync(
                    request,
                    cancellationToken);

                return Results.Created($"/products/{product.Id}", product);
            });

            // Get product by id
            app.MapGet("/products/{id}", async (int id,IProductService service,CancellationToken cancellationToken) =>
            {
                var product = await service.GetProductByIdAsync(
                    id,
                    cancellationToken);

                if (product == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(product);
            });

            // Get all products
            app.MapGet("/products", async (IProductService service,CancellationToken cancellationToken) =>
            {
                var products = await service.GetAllProductsAsync(
                    cancellationToken);

                if (products == null || products.Count == 0)
                {
                    return Results.NotFound(
                        new { message = "No products found." });
                }

                return Results.Ok(products);
            });

            // Update product by id
            app.MapPut("/products/{id}", async (int id,UpdateProductRequest request, IProductService service,CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrEmpty(request.Name) || request.Price < 0)
                {
                    return Results.BadRequest("Invalid product data.");
                }

                var updatedProduct = await service.UpdateProductbyIdAsync( id,request,cancellationToken);
                if (updatedProduct == null)
                {
                    return Results.NotFound(
                        new
                        {
                            message = $"Product with id {id} was not found."
                        });
                }

                return Results.Ok(updatedProduct);
            });

            // Delete product by id
            app.MapDelete("/products/{id}", async ( int id,IProductService service,CancellationToken cancellationToken) =>
            {
                var deleted = await service.DeleteProductByIdAsync(id,cancellationToken);
                if (!deleted)
                {
                    return Results.NotFound(
                        new
                        {
                            message = $"Product with id {id} was not found."
                        });
                }

                return Results.NoContent();
            });

            app.Run();
        }
    }
}