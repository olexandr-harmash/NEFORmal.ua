namespace NEFORmal.ua.Identity.Api.Services;

public class JwtTokenOptions
{
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public double Expires { get; init; }
}
