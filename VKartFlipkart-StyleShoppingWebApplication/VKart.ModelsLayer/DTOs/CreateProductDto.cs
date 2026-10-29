using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace VKart.ModelsLayer.DTOs;

public class CreateProductDto
{
    [Required, MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(0.01, 9999999)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    public IFormFile? ImageFile { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}