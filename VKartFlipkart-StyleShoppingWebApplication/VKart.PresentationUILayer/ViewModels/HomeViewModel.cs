using VKart.ModelsLayer.DTOs;

namespace VKart.PresentationLayer.ViewModels;

public class HomeViewModel
{
    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

    // Filter / sort state (kept so the form retains values)
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public string? SortBy { get; set; }
}