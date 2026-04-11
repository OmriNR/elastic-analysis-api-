using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain;

public class Order
{
    [DataMember(Name = "order_id")]
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; }
    
    [DataMember(Name = "timestamp")]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    
    [DataMember(Name = "customer")]
    [JsonPropertyName("customer")]
    public UserProperties Customer { get; set; }
    
    [DataMember(Name = "total_amount")]
    [JsonPropertyName("total_amount")]
    public double TotalAmount { get; set; }
    
    [DataMember(Name = "payment_method")]
    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; }
    
    [DataMember(Name = "discount_applied")]
    [JsonPropertyName("discount_applied")]
    public bool DiscountApplied { get; set; }
    
    [DataMember(Name = "items")]
    [JsonPropertyName("items")]
    public List<Product> Items { get; set; }
}