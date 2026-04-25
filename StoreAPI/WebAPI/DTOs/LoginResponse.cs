using Domain;

namespace WebAPI.DTOs;

public record LoginResponse(User User, string Token);
