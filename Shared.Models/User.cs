using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Shared.Models;

public class User
{
    [DataMember(Name = "user_id")]
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
    
    [DataMember(Name = "user_name")]
    [JsonPropertyName("user_name")]
    public string UserName { get; set; }
    
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