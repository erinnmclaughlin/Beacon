using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;

namespace Beacon.API.Persistence.Extensions;

public static class ProjectQueryableExtensions
{
    public static IQueryable<Project> WithCode(this IQueryable<Project> query, ProjectCode code)
    {
        return query.Where(x => x.ProjectCode.CustomerCode == code.CustomerCode && 
                                x.ProjectCode.Date == code.Date &&
                                x.ProjectCode.Suffix == code.Suffix);
    }
}