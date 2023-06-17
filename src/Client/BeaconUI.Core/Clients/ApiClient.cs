﻿using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using Beacon.Common.Projects;
using Beacon.Common.Projects.Requests;
using ErrorOr;

namespace BeaconUI.Core.Clients;

public sealed class ApiClient : ApiClientBase
{
    public ApiClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

    public async Task<ErrorOr<SessionInfoDto>> GetSessionInfo()
    {
        return await GetAsync<SessionInfoDto>("api/session");
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

    public async Task<ErrorOr<LaboratoryDto[]>> GetMyLaboratories()
    {
        return await GetAsync<LaboratoryDto[]>("api/laboratories");
    }

    public async Task<ErrorOr<Success>> CreateLaboratory(CreateLaboratoryRequest request)
    {
        return await PostAsync("api/laboratories", request);
    }

    public async Task<ErrorOr<Success>> LoginToLaboratory(Guid labId)
    {
        return await PostAsync($"api/laboratories/{labId}/login", null);
    }

    public async Task<ErrorOr<LaboratoryMemberDto[]>> GetLaboratoryMembers()
    {
        return await GetAsync<LaboratoryMemberDto[]>("api/members");
    }

    public async Task<ErrorOr<Success>> UpdateMembershipType(Guid memberId, UpdateMembershipTypeRequest request)
    {
        return await PutAsync($"api/members/{memberId}/membershipType", request);
    }

    public async Task<ErrorOr<ProjectDto[]>> GetProjects()
    {
        return await GetAsync<ProjectDto[]>("api/projects");
    }

    public async Task<ErrorOr<ProjectDto>> CreateProject(CreateProjectRequest request)
    {
        return await PostAsync<ProjectDto>("api/projects", request);
    }
}
