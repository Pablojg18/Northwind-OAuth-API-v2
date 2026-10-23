using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APIWeb.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required, MaxLength(40)]
    public string ProductName { get; set; } = string.Empty;

    public int? SupplierId { get; set; }

    public int? CategoryId { get; set; }

    [MaxLength(20)]
    public string? QuantityPerUnit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitPrice { get; set; }

    public int? UnitsInStock { get; set; }

    public int? UnitsOnOrder { get; set; }

    public int? ReorderLevel { get; set; }

    public bool Discontinued { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }

    [JsonIgnore]
    public Supplier? Supplier { get; set; }
}
