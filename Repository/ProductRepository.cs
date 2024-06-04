using CatalogAPI.Context;
using CatalogAPI.Models;
using CatalogAPI.Pagination;
using X.PagedList;

namespace CatalogAPI.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
	public ProductRepository(AppDbContext context) : base(context)
	{
	}

	public async Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParams)
	{
		var products = await GetAllAsync();

		var orderedProducts = products.OrderBy(p => p.Id).AsQueryable();

		var result = await orderedProducts.ToPagedListAsync(productsParams.PageNumber, productsParams.PageSize);
		return result;
	}

	public async Task<IEnumerable<Product>> GetProductsByCategoriesAsync(int id)
	{
		var products = await GetAllAsync();

		var filteredProducts = products.Where(c => c.CategoryID == id);

		return filteredProducts;
	}

	public async Task<IPagedList<Product>> GetProductsFilterPriceAsync(ProductFilterPrice productFilterParams)
	{
		var products = await GetAllAsync();

		if (productFilterParams.Price.HasValue && !string.IsNullOrEmpty(productFilterParams.PriceRequirements))
		{
			if (productFilterParams.PriceRequirements.Equals("maior", StringComparison.OrdinalIgnoreCase))
			{
				products = products.Where(p => p.Price > productFilterParams.Price.Value).OrderBy(p => p.Price);
			}
			if (productFilterParams.PriceRequirements.Equals("menor", StringComparison.OrdinalIgnoreCase))
			{
				products = products.Where(p => p.Price < productFilterParams.Price.Value).OrderBy(p => p.Price);
			}
			if (productFilterParams.PriceRequirements.Equals("igual", StringComparison.OrdinalIgnoreCase))
			{
				products = products.Where(p => p.Price == productFilterParams.Price.Value).OrderBy(p => p.Price);
			}
		}

		var filteredProducts = await products.ToPagedListAsync(productFilterParams.PageNumber, productFilterParams.PageSize);

		return filteredProducts;
	}
}
