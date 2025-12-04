using App.Clinic.ViewModels;

namespace App.Clinic.Views;

public partial class AppointmentPage : ContentPage
{
    public AppointmentPage()
    {
        InitializeComponent();
        BindingContext = new AppointmentManagementViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AppointmentManagementViewModel vm)
        {
            vm.Refresh();
        }
    }
}
