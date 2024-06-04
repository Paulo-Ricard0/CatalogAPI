using CatalogAPI.Models;
using CatalogAPI.Pagination;
using X.PagedList;

namespace CatalogAPI.Repository;

public interface ICategoryRepository : IRepository<Category>
{
	Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParams);
	Task<IPagedList<Category>> GetCategoriesFilterNameAsync(CategoriesFilterName categoriesParams);
};
