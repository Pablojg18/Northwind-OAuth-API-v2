using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIWeb.Models;

public class Shipper
{
    public int ShipperId { get; set; }

    [Required, MaxLength(40)]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(24)]
    public string? Phone { get; set; }

    [JsonIgnore]
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
