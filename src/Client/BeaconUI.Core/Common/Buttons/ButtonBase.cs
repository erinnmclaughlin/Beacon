using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Common.Buttons;

public abstract class ButtonBase : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? AdditionalAttributes { get; set; }

    protected virtual bool IsDisabled { get; }

    protected abstract Task Click();
}
