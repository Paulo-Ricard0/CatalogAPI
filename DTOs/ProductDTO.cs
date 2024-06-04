using System.ComponentModel.DataAnnotations;

namespace CatalogAPI.DTOs;

public class ProductDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string? Name { get; set; }

    [Required]
    [StringLength(300)]
    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    [StringLength(300)]
    public string? ImageUrl { get; set; }

    public int CategoryID { get; set; }
}
