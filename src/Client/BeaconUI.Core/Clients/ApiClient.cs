using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using ErrorOr;

namespace BeaconUI.Core.Clients;

internal sealed class ApiClient : ApiClientBase
{
    public ApiClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

    public async Task<ErrorOr<AuthUserDto>> GetCurrentUser()
    {
        return await GetAsync<AuthUserDto>("api/me");
    }

    public async Task<ErrorOr<Success>> Login(LoginRequest request)
    {
        return await PostAsync("api/auth/login", request);
    }

    public async Task<ErrorOr<Success>> Register(RegisterRequest request)
    {
        return await PostAsync("api/auth/register", request);
    }

    public async Task<ErrorOr<Success>> Logout()
    {
        return await GetAsync("api/auth/logout");
    }

    public async Task<ErrorOr<Success>> SendEmailInvitation(InviteLabMemberRequest request)
    {
        return await PostAsync("api/invitations", request);
    }

    public async Task<ErrorOr<Success>> AcceptEmailInvitation(Guid inviteId, Guid emailId)
    {
        return await GetAsync($"api/invitations/{inviteId}/accept?emailId={emailId}");
    }

    public async Task<ErrorOr<Success>> CreateLaboratory(CreateLaboratoryRequest request)
    {
        return await PostAsync("api/laboratories", request);
    }

    public async Task<ErrorOr<Success>> LoginToLaboratory(Guid labId)
    {
        return await PostAsync($"api/laboratories/{labId}", null);
    }

    public async Task<ErrorOr<LaboratoryDetailDto>> GetCurrentLaboratory()
    {
        return await GetAsync<LaboratoryDetailDto>("api/laboratories/current");
    }

    public async Task<ErrorOr<Success>> UpdateMembershipType(Guid memberId, UpdateMembershipTypeRequest request)
    {
        return await PutAsync($"members/{memberId}/membershipType", request);
    }
}
