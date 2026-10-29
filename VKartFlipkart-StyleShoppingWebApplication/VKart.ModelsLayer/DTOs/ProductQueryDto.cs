namespace VKart.ModelsLayer.DTOs;

public class ProductQueryDto
{
    public string? Search { get; set; }       // product name contains
    public int? CategoryId { get; set; }      // filter by category
    public string? SortBy { get; set; }       // "price_asc","price_desc","name_asc","name_desc","newest"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}