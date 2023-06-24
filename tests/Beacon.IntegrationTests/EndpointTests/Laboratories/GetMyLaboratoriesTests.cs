using Beacon.Common.Laboratories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class GetMyLaboratoriesTests : EndpointTestBase
{
    public GetMyLaboratoriesTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetMyLaboratories_ReturnsOnlyCurrentUsersLabs()
    {
        AddTestAuthorization(Common.Memberships.LaboratoryMembershipType.Admin);
        
        var client = CreateClient(db =>
        {
            db.Laboratories.Add(new App.Entities.Laboratory
            {
                Id = Guid.NewGuid(),
                Name = "Other Lab"
            });

            db.SaveChanges();
        });

        var response = await client.GetAsync("api/laboratories");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var labs = await response.Content.ReadFromJsonAsync<LaboratoryDto[]>(JsonSerializerOptions);
        labs.Should().ContainSingle().Which.Id.Should().Be(TestData.DefaultLaboratory.Id);
    }
}
