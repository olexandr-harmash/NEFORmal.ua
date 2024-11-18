using NEFORmal.ua.Identity.Api.Interfaces;

namespace NEFORmal.ua.Identity.Api.Apis;

public class IdentityServices(IAuthorizationService AuthorizationService)
{
    public IAuthorizationService AuthorizationService { get; init; } = AuthorizationService;
}