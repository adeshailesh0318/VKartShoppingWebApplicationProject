using VKart.ModelsLayer.Enums;

namespace VKart.ModelsLayer.DTOs;



public class OrderItemDto
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } // <--- ADD THIS LINE
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

// ... OrderDto remains the same ...
public class OrderDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string OrderStatusText => OrderStatus.ToString();
    public string ShippingAddress { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public IList<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}