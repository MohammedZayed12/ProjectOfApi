using Project.Dto;

namespace Project
{
    public interface IProductService
    {
        Task<ProductResponse> CreateProductAsync(CreateProductRequest request,CancellationToken cancellationToken);

         Task<ProductResponse?> UpdateProductbyIdAsync(int id,UpdateProductRequest request,CancellationToken cancellationToken);

         Task<ProductResponse?> GetProductByIdAsync( int id, CancellationToken cancellationToken);

         Task<List<ProductResponse>> GetAllProductsAsync( CancellationToken cancellationToken);

         Task<bool> DeleteProductByIdAsync( int id,CancellationToken cancellationToken);
    }
}