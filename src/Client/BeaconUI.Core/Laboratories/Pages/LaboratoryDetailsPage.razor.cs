using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Requests.Projects;
using BeaconUI.Core.Common.Http;
using ChartJs.Blazor;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.Util;
using ErrorOr;
using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace BeaconUI.Core.Laboratories.Pages;

public partial class LaboratoryDetailsPage
{
    private LineConfig _config = default!;
    private Chart _chart = default!;

    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    private ErrorOr<GetProjectTypeFrequencyRequest.Series[]>? ProjectTypeFrequencies { get; set; }
    private ErrorOr<PagedList<LaboratoryEventDto>>? Events { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _config = new LineConfig
        {
            Options = new LineOptions
            {
                Responsive = true,
                Title = new OptionsTitle
                {
                    Display = true,
                    Text = "Project Trends"
                },
                Tooltips = new Tooltips
                {
                    Mode = InteractionMode.Nearest,
                    Intersect = true
                },
                Hover = new Hover
                {
                    Mode = InteractionMode.Nearest,
                    Intersect = true
                },
                Scales = new Scales
                {
                    XAxes = new List<CartesianAxis>
                    {
                        new CategoryAxis
                        {
                            ScaleLabel = new ScaleLabel
                            {
                                LabelString = "Month"
                            }
                        }
                    },
                    YAxes = new List<CartesianAxis>
                    {
                        new LinearCartesianAxis
                        {
                            Display = AxisDisplay.True,
                            ScaleLabel = new ScaleLabel
                            {
                                LabelString = "# Projects"
                            }
                        }
                    }
                }
            }
        };

        var start = DateOnly.FromDateTime(DateTime.Today).AddYears(-1);
        start = new DateOnly(start.Year, start.Month, 1);
        for (int i = 0; i < 12; i++)
        {
            _config.Data.Labels.Add(start.AddMonths(i).ToString("MMM yyyy"));
        }

        await Task.WhenAll(LoadAnalytics(), LoadEvents());

        if (ProjectTypeFrequencies.HasValue && !ProjectTypeFrequencies.Value.IsError)
        {
            var seriesOptions = ProjectTypeFrequencies.Value.Value
                .Where(v => v.ProjectCountByDate.Any(d => d.Value > 0))
                .OrderByDescending(v => v.ProjectCountByDate.Sum(d => d.Value));

            foreach (var series in seriesOptions)
            {
                _config.Data.Datasets.Add(new LineDataset<int>(series.ProjectCountByDate.Values)
                {
                    Label = series.ProjectType,
                    BorderColor = ColorUtil.RandomColorString(),
                    BackgroundColor = "transparent"
                });
            }
        }

        await _chart.Update();
    }

    private async Task LoadAnalytics()
    {
        ProjectTypeFrequencies = await ApiClient.SendAsync(new GetProjectTypeFrequencyRequest());
    }

    private async Task LoadEvents()
    {
        Events = await ApiClient.SendAsync(new GetLaboratoryEventsRequest
        {
            MinEnd = DateTime.UtcNow,
            MaxStart = DateTime.UtcNow.AddDays(7)
        });
    }
}
