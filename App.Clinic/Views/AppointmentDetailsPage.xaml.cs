using App.Clinic.ViewModels;

namespace App.Clinic.Views;

public partial class AppointmentDetailsPage : ContentPage
{
    public AppointmentDetailsPage()
    {
        System.Diagnostics.Debug.WriteLine("[AppointmentDetailsPage] Constructor called");
        InitializeComponent();
        BindingContext = new AppointmentDetailsViewModel();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        System.Diagnostics.Debug.WriteLine("[AppointmentDetailsPage] OnNavigatedTo called");

        if (BindingContext is AppointmentDetailsViewModel viewModel)
        {
            await viewModel.Load();
        }
    }
}
