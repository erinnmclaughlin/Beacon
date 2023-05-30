using Beacon.Common.Auth;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Laboratories.Services;

public sealed class CurrentUserMembershipProvider : IDisposable
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ISender _sender;

    private List<LaboratoryMembershipDto>? _memberships;

    public Action<List<LaboratoryMembershipDto>>? MembershipsChanged;

    public CurrentUserMembershipProvider(AuthenticationStateProvider authStateProvider, ISender sender)
    {
        _authStateProvider = authStateProvider;
        _sender = sender;

        _authStateProvider.AuthenticationStateChanged += HandleAuthenticationStateChanged;
    }

    public void Dispose()
    {
        _authStateProvider.AuthenticationStateChanged -= HandleAuthenticationStateChanged;
    }

    public async Task<List<LaboratoryMembershipDto>> GetMembershipsAsync()
    {
        if (_memberships == null)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            _memberships = await GetMemberships(authState.User.ToUserDto()?.Id);
        }

        return _memberships;
    }

    public async Task RefreshState()
    {
        _memberships = null;
        MembershipsChanged?.Invoke(await GetMembershipsAsync());
    }

    private async void HandleAuthenticationStateChanged(Task<AuthenticationState> _)
    {
        await RefreshState();
    }

    private async Task<List<LaboratoryMembershipDto>> GetMemberships(Guid? currentUserId)
    {
        if (currentUserId == null)
            return new();

        var result = await _sender.Send(new GetLaboratoryMembershipsByUserIdRequest
        {
            UserId = currentUserId.Value
        });

        return result.Value;
    }
}
