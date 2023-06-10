using Beacon.Common.Laboratories;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Laboratories;

public partial class LaboratoryDetailsPage
{
    [CascadingParameter] public required LaboratoryDetailDto Details { get; set; }

    [Parameter] public required Guid Id { get; set; }
}