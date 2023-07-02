using Beacon.Common.Models;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Invitations;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Requests.Memberships;
using Beacon.Common.Requests.Projects;
using Beacon.Common.Requests.Projects.Contacts;
using Beacon.Common.Requests.Projects.SampleGroups;
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

    public async Task<ErrorOr<CurrentUserDto>> GetCurrentUser()
    {
        return await _httpClientFactory.GetAsync<CurrentUserDto>("api/users/current");
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

    public async Task<ErrorOr<Success>> SendEmailInvitation(CreateEmailInvitationRequest request)
    {
        return await _httpClientFactory.PostAsync("api/invitations", request);
    }

    public async Task<ErrorOr<Success>> AcceptEmailInvitation(Guid emailId)
    {
        return await _httpClientFactory.GetAsync($"api/invitations/{emailId}/accept");
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

    public async Task<ErrorOr<LaboratoryMemberDto[]>> GetLaboratoryMembers()
    {
        return await _httpClientFactory.GetAsync<LaboratoryMemberDto[]>($"api/memberships");
    }

    public async Task<ErrorOr<Success>> UpdateMembershipType(UpdateMembershipRequest request)
    {
        return await _httpClientFactory.PutAsync($"api/memberships", request);
    }

    public async Task<ErrorOr<ProjectDto[]>> GetProjects()
    {
        return await _httpClientFactory.GetAsync<ProjectDto[]>("api/projects");
    }

    public async Task<ErrorOr<ProjectDto>> GetProject(ProjectCode projectCode)
    {
        return await _httpClientFactory.GetAsync<ProjectDto>($"api/projects/{projectCode}");
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

    public async Task<ErrorOr<Success>> AddProjectContact(CreateProjectContactRequest request)
    {
        return await _httpClientFactory.PostAsync($"api/projects/{request.ProjectId}/contacts", request);
    }

    public async Task<ErrorOr<Success>> UpdateProjectContact(UpdateProjectContactRequest request)
    {
        return await _httpClientFactory.PutAsync($"api/projects/{request.ProjectId}/contacts/{request.ContactId}", request);
    }

    public async Task<ErrorOr<Success>> DeleteProjectContact(DeleteProjectContactRequest request)
    {
        return await _httpClientFactory.DeleteAsync($"api/projects/{request.ProjectId}/contacts/{request.ContactId}");
    }

    public async Task<ErrorOr<ProjectContactDto[]>> GetProjectContacts(Guid projectId)
    {
        return await _httpClientFactory.GetAsync<ProjectContactDto[]>($"api/projects/{projectId}/contacts");
    }

    public async Task<ErrorOr<Success>> CreateSampleGroup(CreateSampleGroupRequest request)
    {
        return await _httpClientFactory.PostAsync($"api/projects/{request.ProjectId}/sample-groups", request);
    }

    public async Task<ErrorOr<SampleGroupDto[]>> GetProjectSampleGroups(Guid projectId)
    {
        return await _httpClientFactory.GetAsync<SampleGroupDto[]>($"api/projects/{projectId}/sample-groups");
    }

    public async Task<ErrorOr<Success>> UpdateLeadAnalyst(UpdateLeadAnalystRequest request)
    {
        return await _httpClientFactory.PutAsync($"api/projects/{request.ProjectId}/analyst", request);
    }
}
