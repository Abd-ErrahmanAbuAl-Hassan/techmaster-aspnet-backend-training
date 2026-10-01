using Task_06___API_Standards___Refactor_Pack.DTOs;

namespace Task_06___API_Standards___Refactor_Pack.Services
{
    public interface IProductsService
    {
        ApiResponse<PagedResult<ProductResponse>> GetProducts(int page = 1 , int pageSize = 10);
        ApiResponse<ProductResponse> GetProductById(int id);
        ApiResponse<ProductResponse> CreateProduct(CreateProductRequest model);

    }
}
