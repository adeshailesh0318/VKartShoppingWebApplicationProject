public class CartDto
{
    public int CartId { get; set; }
    public IList<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    public decimal SubTotal { get; set; }
    public int TotalItems { get; set; }
}