using Beacon.Common.Auth;
using Beacon.Common.Laboratories;
using BeaconUI.Core.Clients;
using ErrorOr;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Services;

internal sealed class BeaconAuthStateProvider : AuthenticationStateProvider
{
    private readonly ApiClient _apiClient;

    public ClaimsPrincipal? CurrentUser { get; private set; }

    public BeaconAuthStateProvider(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (CurrentUser == null)
        {
            var errorOrCurrentUser = await _apiClient.GetCurrentUser();
            var errorOrCurrentLab = await _apiClient.GetCurrentLaboratory();
            CurrentUser = GetClaimsPrincipal(errorOrCurrentUser, errorOrCurrentLab);
        }

        return new AuthenticationState(CurrentUser);
    }

    public void NotifyAuthenticationStateChanged()
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
