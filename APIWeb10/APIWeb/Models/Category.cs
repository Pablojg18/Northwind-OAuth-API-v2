using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIWeb.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required, MaxLength(15)]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
