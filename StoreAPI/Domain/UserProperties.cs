using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Domain;

[Owned]
public class UserProperties
{
    [DataMember(Name = "user_name")]
    [JsonPropertyName("user_name")]
    public string UserName { get; set; }
    
    [DataMember(Name = "age")]
    [JsonPropertyName("age")]
    public int Age { get; set; }
    
    [DataMember(Name = "gender")]
    [JsonPropertyName("gender")]
    public string Gender { get; set; }
    
    [DataMember(Name = "location")]
    [JsonPropertyName("location")]
    public GeoProperties Location { get; set; }
    
    [DataMember(Name = "created_at")]
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}

public class GeoProperties
{
    [DataMember(Name = "city")]
    [JsonPropertyName("city")]
    public string City { get; set; }
    
    [DataMember(Name = "country")]
    [JsonPropertyName("country")]
    public string Country { get; set; }
    
    [DataMember(Name = "address")]
    [JsonPropertyName("address")]
    public string Address { get; set; }
    
    [DataMember(Name = "zip_code")]
    [JsonPropertyName("zip_code")]
    public string ZipCode { get; set; }
}