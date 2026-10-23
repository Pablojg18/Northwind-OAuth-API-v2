using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APIWeb.Models;

public class OrderDetail
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public double Discount { get; set; }

    [JsonIgnore]
    public Order? Order { get; set; }

    [JsonIgnore]
    public Product? Product { get; set; }
}
