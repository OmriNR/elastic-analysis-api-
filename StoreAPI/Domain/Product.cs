using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain;

public class Product
{
    [DataMember(Name = "product_id")]
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }
    
    [DataMember(Name = "owner_id")]
    [JsonPropertyName("owner_id")]
    public string OwnerId { get; set; }
    
    [DataMember(Name = "name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [DataMember(Name = "description")]
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [DataMember(Name = "category")]
    [JsonPropertyName("category")]
    public string Category { get; set; }
    
    [DataMember(Name = "sub_category")]
    [JsonPropertyName("sub_category")]
    public string? SubCategory { get; set; }
    
    [DataMember(Name = "price")]
    [JsonPropertyName("price")]
    public double Price { get; set; }
    
    [DataMember(Name = "quantity")]
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}