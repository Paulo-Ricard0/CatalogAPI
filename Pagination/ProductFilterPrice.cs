namespace CatalogAPI.Pagination;

public class ProductFilterPrice : QueryStringParameters
{
	public decimal? Price { get; set; }
	public string? PriceRequirements { get; set; }
}
