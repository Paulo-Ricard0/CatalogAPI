namespace CatalogAPI.DTOs;

public class ProductDTOUpdateResponse
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public decimal Price { get; set; }
	public string? ImageUrl { get; set; }
	public float Stock { get; set; }
	public DateTime RegisterData { get; set; }
	public int CategoryID { get; set; }
}
