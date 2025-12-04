using App.Clinic.ViewModels;

namespace App.Clinic.Views;

public partial class PhysicianDetailsPage : ContentPage
{
    public PhysicianDetailsPage()
    {
        InitializeComponent();
        BindingContext = new PhysicianDetailsViewModel();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is PhysicianDetailsViewModel viewModel)
        {
            await viewModel.Load();
        }
    }
}
