using VKart.ModelsLayer.DTOs;

namespace VKart.PresentationLayer.ViewModels;

public class ProductDetailsViewModel
{
    public ProductDto Product { get; set; } = new();
    public int Quantity { get; set; } = 1;
}