using BeaconUI.Core;
using Microsoft.Extensions.Logging;

namespace BeaconUI.DesktopApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddHttpClient("BeaconApi", options =>
        {
            // TODO: pull this from config
            options.BaseAddress = new Uri("http://localhost:5020");
        });

        builder.Services.AddBeaconUI();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
