using Microsoft.Extensions.Logging;
using Maui.Clinic.ViewModels;
using Maui.Clinic.Views;

namespace Maui.Clinic;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register services
        builder.Services.AddSingleton<PatientViewModel>();
        builder.Services.AddSingleton<PatientView>();
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}