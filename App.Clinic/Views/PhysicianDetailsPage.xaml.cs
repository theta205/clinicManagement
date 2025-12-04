using App.Clinic.ViewModels;

namespace App.Clinic.Views;

public partial class PhysicianDetailsPage : ContentPage
{
    public PhysicianDetailsPage()
    {
        System.Diagnostics.Debug.WriteLine("[PhysicianDetailsPage] Constructor called");
        InitializeComponent();
        BindingContext = new PhysicianDetailsViewModel();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        System.Diagnostics.Debug.WriteLine("[PhysicianDetailsPage] OnNavigatedTo called");

        if (BindingContext is PhysicianDetailsViewModel viewModel)
        {
            await viewModel.Load();
        }
    }
}
