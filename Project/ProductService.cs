using Microsoft.EntityFrameworkCore;
using Project.Dto;

namespace Project
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext db;

        private readonly ILogger<ProductService> _logger;

        public ProductService(AppDbContext  context,ILogger<ProductService> logger)
        {
            db = context;
            _logger = logger;
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request,CancellationToken cancellationToken)
        {
            try
            {
                var product = new Product
                {
                    Price = request.Price,
                    Name = request.Name,
                    CreatedAtUtc = DateTime.UtcNow
                };

                db.Products.Add(product);

                await db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Product created successfully with ID: {ProductId}",
                    product.Id);

                return ToResponse(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while creating the product.");

                throw;
            }
        }

        public async Task<bool> DeleteProductByIdAsync( int id, CancellationToken cancellationToken)
        {
            var product = await db.Products
                .FirstOrDefaultAsync( p => p.Id == id, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning(
                    "Product with ID {ProductId} not found.",
                    id);

                return false;
            }

            db.Products.Remove(product);

            await db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Product with ID {ProductId} deleted successfully.",
                id);

            return true;
        }

        public async Task<List<ProductResponse>> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            var products = await db.Products
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return products
                .Select(p => ToResponse(p))
                .ToList();
        }

        public async Task<ProductResponse?> GetProductByIdAsync(int id,CancellationToken cancellationToken)
        {
            var product = await db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(  p => p.Id == id,cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.",id);

                return null;
            }

            return ToResponse(product);
        }

        public async Task<ProductResponse?> UpdateProductbyIdAsync( int id,UpdateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await db.Products
                    .FirstOrDefaultAsync(
                        p => p.Id == id,
                        cancellationToken);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.",id);

                    return null;
                }

                product.Name = request.Name;
                product.Price = request.Price;

                await db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Product with ID {ProductId} updated successfully.",id);

                return ToResponse(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"An error occurred while updating the product with ID {ProductId}.", id);
                throw;
            }
        }

        private static ProductResponse ToResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CreatedAtUtc = product.CreatedAtUtc
            };
        }
    }
}