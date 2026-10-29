using System.ComponentModel.DataAnnotations;

namespace VKart.ModelsLayer.DTOs;

public class AddToCartDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 999)]
    public int Quantity { get; set; } = 1;
}