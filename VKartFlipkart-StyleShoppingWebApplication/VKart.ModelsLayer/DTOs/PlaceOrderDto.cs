using System.ComponentModel.DataAnnotations;

namespace VKart.ModelsLayer.DTOs;

public class PlaceOrderDto
{
    [Required, MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;
}