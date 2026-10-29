using VKart.ModelsLayer.DTOs;

namespace VKart.PresentationLayer.ViewModels;

public class CartViewModel
{
    public CartDto Cart { get; set; } = new();
    public string? Message { get; set; }
    public string? Error { get; set; }
}