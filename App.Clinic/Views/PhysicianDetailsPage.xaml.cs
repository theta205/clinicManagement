using App.Clinic.ViewModels;

namespace App.Clinic.Views;

public partial class PhysicianDetailsPage : ContentPage
{
    public PhysicianDetailsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is PhysicianDetailsViewModel viewModel)
        {
            // Get physician ID from navigation parameters
            var navigationData = Shell.Current.CurrentState.Location.OriginalString;
            int physicianId = 0;
            
            if (navigationData.Contains("physicianId="))
            {
                var startIndex = navigationData.IndexOf("physicianId=") + 12;
                var endIndex = navigationData.IndexOf("&", startIndex);
                if (endIndex == -1) endIndex = navigationData.Length;
                
                var idString = navigationData.Substring(startIndex, endIndex - startIndex);
                int.TryParse(idString, out physicianId);
            }
            
            await viewModel.LoadPhysician(physicianId);
        }
    }
}
