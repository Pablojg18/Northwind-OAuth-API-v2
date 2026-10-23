using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIWeb.Models;

public class Supplier
{
    public int SupplierId { get; set; }

    [Required, MaxLength(40)]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? ContactName { get; set; }

    [MaxLength(30)]
    public string? ContactTitle { get; set; }

    [MaxLength(60)]
    public string? Address { get; set; }

    [MaxLength(15)]
    public string? City { get; set; }

    [MaxLength(15)]
    public string? Region { get; set; }

    [MaxLength(10)]
    public string? PostalCode { get; set; }

    [MaxLength(15)]
    public string? Country { get; set; }

    [MaxLength(24)]
    public string? Phone { get; set; }

    [MaxLength(24)]
    public string? Fax { get; set; }

    [MaxLength(120)]
    public string? HomePage { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
