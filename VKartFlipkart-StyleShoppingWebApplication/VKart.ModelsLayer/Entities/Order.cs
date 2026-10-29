using System.ComponentModel.DataAnnotations.Schema;
using VKart.ModelsLayer.Enums;

namespace VKart.ModelsLayer.Entities;

public class Order
{
    public int OrderId { get; set; }

    public string UserId { get; set; } = string.Empty;
    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public string ShippingAddress { get; set; } = string.Empty;

    // Navigation
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}