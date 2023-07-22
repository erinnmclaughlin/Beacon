using Beacon.Common.Models;
using Beacon.Common.Requests;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Invitations;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Requests.Memberships;
using Beacon.Common.Requests.Projects;
using Beacon.Common.Requests.Projects.Contacts;
using Beacon.Common.Requests.Projects.SampleGroups;
using Beacon.Common.Services;
using ErrorOr;

namespace BeaconUI.Core.Common.Http;

public static class RequestExtensions
{
    public static async Task<ErrorOr<Success>> SendAsync<TRequest>(this IHttpClientFactory httpClientFactory, TRequest request, CancellationToken ct = default) where TRequest : BeaconRequest<TRequest>
    {
        using var httpClient = httpClientFactory.CreateBeaconClient();
        var response = await request.SendAsync(httpClient, request, ct);
        return await response.ToErrorOrResult(ct);
    }

    public static async Task<ErrorOr<TResult>> SendAsync<TRequest, TResult>(this IHttpClientFactory httpClientFactory, TRequest request, CancellationToken ct = default) where TRequest : BeaconRequest<TRequest, TResult>
    {
        using var httpClient = httpClientFactory.CreateBeaconClient();
        var response = await request.SendAsync(httpClient, request, ct);
        return await response.ToErrorOrResult<TResult>(ct);
    }
}

public sealed class ApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ErrorOr<SessionContext>> GetCurrentUser()
    {
        return await _httpClientFactory.SendAsync<GetSessionContextRequest, SessionContext>(new GetSessionContextRequest());
    }

    public async Task<ErrorOr<Success>> Login(LoginRequest request)
    {
        return await _httpClientFactory.SendAsync(request);
    }

    public async Task<ErrorOr<Success>> Register(RegisterRequest request)
    {
        return await _httpClientFactory.SendAsync(request);
    }

    public async Task<ErrorOr<Success>> Logout()
    {
        return await _httpClientFactory.SendAsync(new LogoutRequest());
    }

    public async Task<ErrorOr<Success>> SendEmailInvitation(CreateEmailInvitationRequest request)
    {
        return await _httpClientFactory.SendAsync(request);
    }

    public async Task<ErrorOr<Success>> AcceptEmailInvitation(Guid emailId)
    {
        return await _httpClientFactory.SendAsync(new AcceptEmailInvitationRequest { EmailInvitationId = emailId });
    }

    public async Task<ErrorOr<LaboratoryDto[]>> GetMyLaboratories()
    {
        return await _httpClientFactory.SendAsync<GetMyLaboratoriesRequest, LaboratoryDto[]>(new ());
    }

    public async Task<ErrorOr<Success>> CreateLaboratory(CreateLaboratoryRequest request)
    {
        return await _httpClientFactory.SendAsync(request);
    }

    public async Task<ErrorOr<LaboratoryDto>> GetCurrentLaboratory()
    {
        return await _httpClientFactory.SendAsync<GetCurrentLaboratoryRequest, LaboratoryDto>(new GetCurrentLaboratoryRequest());
    }

    public async Task<ErrorOr<Success>> SetCurrentLaboratory(Guid id)
    {
        return await _httpClientFactory.SendAsync(new SetCurrentLaboratoryRequest
        {
            LaboratoryId = id
        });
    }

    public async Task<ErrorOr<LaboratoryMemberDto[]>> GetLaboratoryMembers()
    {
        return await _httpClientFactory.SendAsync<GetMembershipsRequest, LaboratoryMemberDto[]>(new GetMembershipsRequest());
    }

    public async Task<ErrorOr<Success>> UpdateMembershipType(UpdateMembershipRequest request)
    {
        return await _httpClientFactory.SendAsync(request);
    }

    public async Task<ErrorOr<ProjectDto[]>> GetProjects()
    {
        return await _httpClientFactory.SendAsync<GetProjectsRequest, ProjectDto[]>(new GetProjectsRequest());
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
