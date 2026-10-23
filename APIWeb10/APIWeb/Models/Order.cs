using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APIWeb.Models;

public class Order
{
    public int OrderId { get; set; }

    [MaxLength(5)]
    public string? CustomerId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? RequiredDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public int? ShipVia { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Freight { get; set; }

    [MaxLength(40)]
    public string? ShipName { get; set; }

    [MaxLength(60)]
    public string? ShipAddress { get; set; }

    [MaxLength(15)]
    public string? ShipCity { get; set; }

    [MaxLength(15)]
    public string? ShipRegion { get; set; }

    [MaxLength(10)]
    public string? ShipPostalCode { get; set; }

    [MaxLength(15)]
    public string? ShipCountry { get; set; }

    [JsonIgnore]
    public Customer? Customer { get; set; }

    [JsonIgnore]
    public Employee? Employee { get; set; }

    [JsonIgnore]
    public Shipper? Shipper { get; set; }

    [JsonIgnore]
    public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
}
