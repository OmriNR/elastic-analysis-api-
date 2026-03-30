using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain;

public class Relation
{
    [DataMember(Name = "relation_id")]
    [JsonPropertyName("relation_id")]
    public string RelationId { get; set; }
    
    [DataMember(Name = "relation_type")]
    [JsonPropertyName("relation_type")]
    public RelationTypes RelationType { get; set; }
    
    [DataMember(Name = "relation_status")]
    [JsonPropertyName("relation_status")]
    public RelationStatuses RelationStatus { get; set; }
    
    [DataMember(Name = "product_id")]
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; }
    
    [DataMember(Name = "user_id")]
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
    
    [DataMember(Name = "total_amount")]
    [JsonPropertyName("total_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? TotalAmount { get; set; }
    
    [DataMember(Name = "discount_percentage")]
    [JsonPropertyName("discount_percentage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? DiscountPercentage { get; set; }
    
    [DataMember(Name = "discount_expire_date")]
    [JsonPropertyName("discount_expire_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? DiscountExpireDate { get; set; }
}