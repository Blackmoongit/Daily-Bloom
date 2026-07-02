using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DailyBloom.Services;

/// <summary>
/// Keeps track of who is logged in for the current browser tab.
/// We use ProtectedSessionStorage (encrypted browser sessionStorage) instead of
/// classic cookie auth, because in interactive Blazor Server, headers can't be
/// changed once the page has started rendering. This keeps things simple and
/// still keeps the session private to the user's browser tab.
/// </summary>
public class CurrentUserService : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    private ClaimsPrincipal? _cachedPrincipal;
    public int? CurrentUserId { get; private set; }
    public string? CurrentUserName { get; private set; }

    /// <summary>Raised when the user's profile (name/picture) is updated,
    /// so other components like the sidebar can refresh without a full reload.</summary>
    public event Action? ProfileUpdated;
    public void NotifyProfileUpdated() => ProfileUpdated?.Invoke();

    public CurrentUserService(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Once we've confirmed the logged-in user once, reuse that result for the
        // rest of the circuit instead of re-reading session storage every time.
        // This avoids a race where a concurrent JS interop call (e.g. a file
        // upload in progress) makes the storage read fail and gets misread as
        // "logged out".
        if (_cachedPrincipal != null)
        {
            return new AuthenticationState(_cachedPrincipal);
        }

        try
        {
            var result = await _sessionStorage.GetAsync<int>("userId");
            var nameResult = await _sessionStorage.GetAsync<string>("userName");
            if (result.Success && result.Value != 0)
            {
                CurrentUserId = result.Value;
                CurrentUserName = nameResult.Value;
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, result.Value.ToString()),
                    new Claim(ClaimTypes.Name, nameResult.Value ?? "")
                }, "DailyBloom");
                _cachedPrincipal = new ClaimsPrincipal(identity);
                return new AuthenticationState(_cachedPrincipal);
            }
        }
        catch
        {
            // storage not yet available during prerender, or a concurrent JS
            // interop call is in progress; don't cache this — just try again
            // next time someone asks.
        }
        return new AuthenticationState(_anonymous);
    }

    public async Task SignInAsync(int userId, string name)
    {
        CurrentUserId = userId;
        CurrentUserName = name;
        await _sessionStorage.SetAsync("userId", userId);
        await _sessionStorage.SetAsync("userName", name);

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, name)
        }, "DailyBloom");
        _cachedPrincipal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedPrincipal)));
    }

    public async Task SignOutAsync()
    {
        CurrentUserId = null;
        CurrentUserName = null;
        _cachedPrincipal = null;
        await _sessionStorage.DeleteAsync("userId");
        await _sessionStorage.DeleteAsync("userName");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
