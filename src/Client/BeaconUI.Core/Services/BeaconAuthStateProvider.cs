using Beacon.Common.Auth;
using Beacon.Common.Laboratories;
using BeaconUI.Core.Clients;
using ErrorOr;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Services;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly AuthClient _authClient;
    private readonly LabClient _labClient;

    public ClaimsPrincipal? CurrentUser { get; private set; }

    public BeaconAuthStateProvider(AuthClient authClient, LabClient labClient)
    {
        _authClient = authClient;
        _authClient.OnChange += HandleAuthenticationStateChanged;
        _labClient = labClient;
    }

    public void Dispose()
    {
        _authClient.OnChange -= HandleAuthenticationStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (CurrentUser == null)
        {
            var errorOrCurrentUser = await _authClient.GetCurrentUserAsync();
            var errorOrCurrentLab = await _labClient.GetLaboratoryDetails();
            CurrentUser = GetClaimsPrincipal(errorOrCurrentUser, errorOrCurrentLab);
        }

        return new AuthenticationState(CurrentUser);
    }

    private void HandleAuthenticationStateChanged()
    {
        CurrentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsPrincipal AnonymousUser { get; } = new ClaimsPrincipal(new ClaimsIdentity());

    private static ClaimsPrincipal GetClaimsPrincipal(ErrorOr<AuthUserDto> errorOrUser, ErrorOr<LaboratoryDetailDto> errorOrLab)
    {
        if (errorOrUser.IsError)
            return AnonymousUser;

        var identity = new ClaimsIdentity("AuthCookie");

        var user = errorOrUser.Value;

        identity.AddClaims(new[]
        {
            new Claim(BeaconClaimTypes.UserId, user.Id.ToString()),
            new Claim(BeaconClaimTypes.DisplayName, user.DisplayName),
            new Claim(BeaconClaimTypes.Email, user.EmailAddress)
        });

        if (!errorOrLab.IsError)
        {
            identity.AddClaims(new[]
            {
                new Claim(BeaconClaimTypes.LabId, errorOrLab.Value.Id.ToString()),
                new Claim(BeaconClaimTypes.MembershipType, errorOrLab.Value.CurrentUserMembershipType.ToString())
            });
        }

        return new ClaimsPrincipal(identity);
    }
}
