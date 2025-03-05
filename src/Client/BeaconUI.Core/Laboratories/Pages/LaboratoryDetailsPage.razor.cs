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

namespace BeaconUI.Core.Laboratories.Pages;

public partial class LaboratoryDetailsPage
{
    private LineConfig _config = null!;
    private Chart _chart = null!;

    [Inject]
    private IApiClient ApiClient { get; set; } = null!;

    private ErrorOr<ProjectInsightDto[]>? ProjectInsights { get; set; }
    private ErrorOr<GetProjectTypeFrequencyRequest.Series[]>? ProjectTypeFrequencies { get; set; }
    private ErrorOr<PagedList<LaboratoryEventDto>>? Events { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _config = new LineConfig
        {
            Options = new LineOptions
            {
                Responsive = true,
                MaintainAspectRatio = false,
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

        var start = DateTime.UtcNow.AddYears(-1).ToDateOnly();
        start = new DateOnly(start.Year, start.Month, 1);
        for (var i = 0; i < 12; i++)
        {
            _config.Data.Labels.Add(start.AddMonths(i).ToString("MMM yyyy"));
        }

        await Task.WhenAll(LoadAnalytics(), LoadEvents());

        if (ProjectTypeFrequencies is { IsError: false })
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

        await LoadInsights();
    }

    private async Task LoadAnalytics()
    {
        ProjectTypeFrequencies = await ApiClient.SendAsync(new GetProjectTypeFrequencyRequest());
    }

    private async Task LoadEvents()
    {
        var now = DateTimeOffset.UtcNow;
        
        Events = await ApiClient.SendAsync(new GetLaboratoryEventsRequest
        {
            MinEnd = now,
            MaxStart = now.AddDays(7)
        });
    }

    private async Task LoadInsights()
    {
        ProjectInsights = await ApiClient.SendAsync(new GetProjectInsightsRequest());
    }
}
