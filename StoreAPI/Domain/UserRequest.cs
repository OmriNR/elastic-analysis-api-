using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain;


public class UserRequest
{
    [DataMember(Name = "requested_user_id")]
    [JsonPropertyName("requested_user_id")]
    public string RequestedUserId { get; init; }
    
    [DataMember(Name = "target_user_id")]
    [JsonPropertyName("target_user_id")]
    public string TargetUserId { get; init; }
}