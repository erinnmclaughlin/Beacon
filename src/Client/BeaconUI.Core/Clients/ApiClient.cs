using Beacon.Common.Auth;
using Beacon.Common.Invitations;
using Beacon.Common.Laboratories;
using Beacon.Common.Memberships;
using Beacon.Common.Projects;
using Beacon.Common.Projects.Requests;
using Beacon.Common.Requests.Auth;
using BeaconUI.Core.Helpers;
using ErrorOr;

namespace BeaconUI.Core.Clients;

public sealed class ApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ErrorOr<SessionInfoDto>> GetSessionInfo()
    {
        return await _httpClientFactory.GetAsync<SessionInfoDto>("api/session");
    }

    public async Task<ErrorOr<Success>> Login(LoginRequest request)
    {
        return await _httpClientFactory.PostAsync("api/auth/login", request);
    }

    public async Task<ErrorOr<Success>> Register(RegisterRequest request)
    {
        return await _httpClientFactory.PostAsync("api/auth/register", request);
    }

    public async Task<ErrorOr<Success>> Logout()
    {
        return await _httpClientFactory.GetAsync("api/auth/logout");
    }

    public async Task<ErrorOr<Success>> SendEmailInvitation(InviteLabMemberRequest request)
    {
        return await _httpClientFactory.PostAsync("api/invitations", request);
    }

    public async Task<ErrorOr<Success>> AcceptEmailInvitation(Guid inviteId, Guid emailId)
    {
        return await _httpClientFactory.GetAsync($"api/invitations/{inviteId}/accept?emailId={emailId}");
    }

    public async Task<ErrorOr<LaboratoryDto[]>> GetMyLaboratories()
    {
        return await _httpClientFactory.GetAsync<LaboratoryDto[]>("api/laboratories");
    }

    public async Task<ErrorOr<Success>> CreateLaboratory(CreateLaboratoryRequest request)
    {
        return await _httpClientFactory.PostAsync("api/laboratories", request);
    }

    public async Task<ErrorOr<LaboratoryDto>> GetCurrentLaboratory()
    {
        return await _httpClientFactory.GetAsync<LaboratoryDto>($"api/laboratories/current");
    }

    public async Task<ErrorOr<LaboratoryMemberDto[]>> GetLaboratoryMembers(Guid labId)
    {
        return await _httpClientFactory.GetAsync<LaboratoryMemberDto[]>($"api/memberships?laboratoryId={labId}");
    }

    public async Task<ErrorOr<Success>> UpdateMembershipType(UpdateMembershipRequest request)
    {
        return await _httpClientFactory.PutAsync($"api/memberships", request);
    }

    public async Task<ErrorOr<ProjectDto[]>> GetProjects()
    {
        return await _httpClientFactory.GetAsync<ProjectDto[]>($"api/projects");
    }

    public async Task<ErrorOr<Success>> CreateProject(CreateProjectRequest request)
    {
        return await _httpClientFactory.PostAsync("api/projects", request);
    }

    public async Task<ErrorOr<Success>> CancelProject(CancelProjectRequest request)
    {
        return await _httpClientFactory.PostAsync($"api/projects/cancel", request);
    }

    public async Task<ErrorOr<Success>> CompleteProject(CompleteProjectRequest request)
    {
        return await _httpClientFactory.PostAsync($"api/projects/complete", request);
    }
}
