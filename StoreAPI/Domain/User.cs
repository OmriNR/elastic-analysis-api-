using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain;

public class User
{
    [DataMember(Name = "user_id")]
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }
    
    [DataMember(Name = "email")]
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [DataMember(Name = "password")]
    [JsonPropertyName("password")]
    public string Password { get; set; }
    
    [DataMember(Name = "is_active")]
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }
    
    [DataMember(Name = "is_admin")]
    [JsonPropertyName("is_admin")]
    public bool IsAdmin { get; set; }
    
    [DataMember(Name = "created_at")]
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}