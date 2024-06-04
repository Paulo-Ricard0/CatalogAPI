using CatalogAPI.Models;
using CatalogAPI.Pagination;
using X.PagedList;

namespace CatalogAPI.Repository;

public interface IProductRepository : IRepository<Product>
{

	Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParams);
	Task<IPagedList<Product>> GetProductsFilterPriceAsync(ProductFilterPrice productFilterParams);
	Task<IEnumerable<Product>> GetProductsByCategoriesAsync(int id);
}
