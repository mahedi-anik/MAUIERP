using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MAUIERP.ApplicationLayer.Common.Interfaces; // Add this
using MAUIERP.Infrastructure.Services; // Add this
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MAUIERP.BlazorUI.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly ICurrentUserService _currentUserService; // Add this
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(
        ILocalStorageService localStorage,
        ICurrentUserService currentUserService) // Inject here
    {
        _localStorage = localStorage;
        _currentUserService = currentUserService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("accessToken");

            if (string.IsNullOrEmpty(token) || !IsTokenValid(token))
            {
                return ClearState();
            }

            return SetState(token);
        }
        catch
        {
            return ClearState();
        }
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await _localStorage.SetItemAsync("accessToken", token);
        var authState = SetState(token);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync("accessToken");
        await _localStorage.RemoveItemAsync("refreshToken");
        await _localStorage.RemoveItemAsync("userInfo");

        var authState = ClearState();
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    private AuthenticationState SetState(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        // SYNC: Update the Infrastructure service with the logged-in user
        if (_currentUserService is CurrentUserService service)
        {
            service.SetUser(user);
        }

        return new AuthenticationState(user);
    }

    private AuthenticationState ClearState()
    {
        if (_currentUserService is CurrentUserService service)
        {
            service.SetUser(_anonymous);
        }
        return new AuthenticationState(_anonymous);
    }

    private bool IsTokenValid(string token)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo > DateTime.UtcNow;
        }
        catch { return false; }
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();

        try
        {
            var token = _tokenHandler.ReadJwtToken(jwt);

            foreach (var claim in token.Claims)
            {
                switch (claim.Type)
                {
                    case "role":
                        claims.Add(new Claim(ClaimTypes.Role, claim.Value));
                        break;

                    case "name":
                        claims.Add(new Claim(ClaimTypes.Name, claim.Value));
                        break;

                    case "sub":
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, claim.Value));
                        break;

                    default:
                        claims.Add(claim);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return claims;
    }
}