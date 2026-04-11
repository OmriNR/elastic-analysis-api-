using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain;

public class Discount
{
    [DataMember(Name = "discount_id")]
    [JsonPropertyName("discount_id")]
    public string DiscountId { get; set; }
    
    [DataMember(Name = "product_id")]
    [JsonPropertyName("product_id")]
    public string ProdcutId { get; set; }
    
    [DataMember(Name = "percentage")]
    [JsonPropertyName("percentage")]
    public double Percentage { get; set; }
    
    [DataMember(Name = "expired_at")]
    [JsonPropertyName("expired_at")]
    public DateTime ExpiredAt { get; set; }
}