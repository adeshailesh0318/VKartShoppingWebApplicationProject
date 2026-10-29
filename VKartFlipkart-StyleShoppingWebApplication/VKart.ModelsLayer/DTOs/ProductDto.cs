using Microsoft.AspNetCore.Http;

namespace VKart.ModelsLayer.DTOs;

public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}