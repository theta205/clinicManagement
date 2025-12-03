using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Library.Clinic.Services;
using App.Clinic.ViewModels;
using App.Clinic.Views;

namespace App.Clinic
{
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
            
            builder.Services.AddSingleton<PatientServiceProxy>();
            builder.Services.AddSingleton<PatientManagementViewModel>();
            builder.Services.AddTransient<PatientDetailsViewModel>();
            builder.Services.AddTransient<PatientManagementPage>();
            builder.Services.AddTransient<PatientDetailsPage>();

            return builder.Build();
        }
    }
}
