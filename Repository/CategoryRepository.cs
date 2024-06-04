using CatalogAPI.Context;
using CatalogAPI.Models;
using CatalogAPI.Pagination;
using X.PagedList;

namespace CatalogAPI.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
	public CategoryRepository(AppDbContext context) : base(context)
	{
	}

	public async Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParams)
	{
		var categories = await GetAllAsync();

		var orderedCategories = categories.OrderBy(c => c.Id).AsQueryable();

		var result = await orderedCategories.ToPagedListAsync(categoriesParams.PageNumber, categoriesParams.PageSize);
		return result;
	}

	public async Task<IPagedList<Category>> GetCategoriesFilterNameAsync(CategoriesFilterName categoriesParams)
	{
		var categories = await GetAllAsync();

		if (!string.IsNullOrEmpty(categoriesParams.Name))
		{
			categories = categories.Where(c => c.Name.Contains(categoriesParams.Name));
		}

		//var filteredCategories = PagedList<Category>.ToPagedList(categories.AsQueryable(), categoriesParams.PageNumber, categoriesParams.PageSize);

		var filteredCategories = await categories.ToPagedListAsync(categoriesParams.PageNumber, categoriesParams.PageSize);
		return filteredCategories;
	}
}
