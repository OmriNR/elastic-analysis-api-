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
    
    [DataMember(Name = "products")]
    [JsonPropertyName("products")]
    public List<string> Products { get; set; }
    
    [DataMember(Name = "percentage")]
    [JsonPropertyName("percentage")]
    public double Percentage { get; set; }
    
    [DataMember(Name = "expired_at")]
    [JsonPropertyName("expired_at")]
    public DateTime ExpiredAt { get; set; }
}