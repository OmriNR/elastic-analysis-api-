using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain;

public class User
{
    [DataMember(Name = "user_id")]
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
    
    [DataMember(Name = "email")]
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [DataMember(Name = "password")]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}