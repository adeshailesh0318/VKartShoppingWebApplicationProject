namespace VKart.ModelsLayer.Entities;

public class Cart
{
    public int CartId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser? User { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}