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
            // Get physician ID from navigation query parameters
            int physicianId = 0;
            
            if (Shell.Current.CurrentState.Location.OriginalString.Contains("physicianId="))
            {
                var queryString = Shell.Current.CurrentState.Location.OriginalString;
                var startIndex = queryString.IndexOf("physicianId=") + 12;
                var endIndex = queryString.IndexOf("&", startIndex);
                if (endIndex == -1) endIndex = queryString.Length;
                
                var idString = queryString.Substring(startIndex, endIndex - startIndex);
                int.TryParse(idString, out physicianId);
            }
            
            // Debug: Show the parsed ID
            await Shell.Current.DisplayAlert("Debug", $"Parsed Physician ID: {physicianId}", "OK");
            
            await viewModel.LoadPhysician(physicianId);
        }
    }
}
