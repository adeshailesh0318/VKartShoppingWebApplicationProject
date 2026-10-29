using System.ComponentModel.DataAnnotations;
using VKart.ModelsLayer.DTOs;

namespace VKart.PresentationLayer.ViewModels;

public class CheckoutViewModel
{
    [Required, MaxLength(500), Display(Name = "Shipping Address")]
    public string ShippingAddress { get; set; } = string.Empty;

    public CartDto Cart { get; set; } = new();
}