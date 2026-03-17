using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain;

public class Customer
{
    [DataMember(Name = "customer_id")]
    [JsonPropertyName("customer_id")]
    public Guid CustomerId { get; set; }
    
    [DataMember(Name = "age")]
    [JsonPropertyName("age")]
    public int Age { get; set; }
    
    [DataMember(Name = "gender")]
    [JsonPropertyName("gender")]
    public string Gender { get; set; }
    
    [DataMember(Name = "city")]
    [JsonPropertyName("city")]
    public string City { get; set; }
    
    [DataMember(Name = "country")]
    [JsonPropertyName("country")]
    public string Country { get; set; }
    
    [DataMember(Name = "created_at")]
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}