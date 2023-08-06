using Beacon.Common;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Common.Pagination;

public partial class PageNumbers
{
    [Parameter, EditorRequired]
    public required PagedList PagedList { get; set; }

    [Parameter]
    public EventCallback<int> OnSelectPage { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? Attributes { get; set; }

    private async Task SelectPage(int page) => await OnSelectPage.InvokeAsync(page);

    private IEnumerable<int> GetPageNumbers(int numberToDisplay = 10)
    {
        var current = PagedList.CurrentPage;
        var start = Math.Max(1, current - 1);
        var end = Math.Min(PagedList.TotalPages, start + numberToDisplay - 1); ;

        while ((start > 1 || end < PagedList.TotalPages) && end - start < numberToDisplay - 1)
        {
            if (end < PagedList.TotalPages)
                end++;
            else if (start > 1)
                start--;
        }

        for (int i = start; i <= end; i++)
            yield return i;
    }
}