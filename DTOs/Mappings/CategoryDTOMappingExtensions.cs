using CatalogAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace CatalogAPI.DTOs.Mappings;

public static class CategoryDTOMappingExtensions
{
    public static CategoryDTO? ToCategoryDTO(this Category category)
    {
        if (category == null)
        {
            return null;
        }

        return new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            ImageUrl = category.ImageUrl,
        };
    }

    public static Category? ToCategory(this CategoryDTO categoryDTO)
    {
        if (categoryDTO == null)
        {
            return null;
        }

        return new Category
        {
            Id = categoryDTO.Id,
            Name = categoryDTO.Name,
            ImageUrl = categoryDTO.ImageUrl,
        };
    }

    public static IEnumerable<CategoryDTO>? ToCategoryDTOList(this IEnumerable<Category> categories)
    {
        if (categories.IsNullOrEmpty())
        {
            return new List<CategoryDTO>();
        }

        return categories.Select(category => new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            ImageUrl = category.ImageUrl,
        }).ToList();
    }
}
