using App.Clinic.ViewModels;

namespace App.Clinic.Views;

public partial class PhysicianManagementPage : ContentPage
{
    public PhysicianManagementPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Refresh the physician list when the page appears
        if (BindingContext is PhysicianManagementViewModel viewModel)
        {
            viewModel.Refresh();
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is PhysicianViewModel physician)
        {
            await Shell.Current.GoToAsync($"PhysicianDetails?physicianId={physician.Id}");
        }
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is PhysicianViewModel physician)
        {
            var viewModel = BindingContext as PhysicianManagementViewModel;
            if (viewModel != null)
            {
                viewModel.SelectedPhysician = physician;
                viewModel.Delete();
            }
        }
    }
}
