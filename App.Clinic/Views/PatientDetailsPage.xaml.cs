using App.Clinic.ViewModels;

namespace App.Clinic.Views;

public partial class PatientDetailsPage : ContentPage
{
    public PatientDetailsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is PatientDetailsViewModel viewModel)
        {
            // Get patient ID from navigation parameters
            var navigationData = Shell.Current.CurrentState.Location.OriginalString;
            int patientId = 0;
            
            if (navigationData.Contains("patientId="))
            {
                var startIndex = navigationData.IndexOf("patientId=") + 10;
                var endIndex = navigationData.IndexOf("&", startIndex);
                if (endIndex == -1) endIndex = navigationData.Length;
                
                var idString = navigationData.Substring(startIndex, endIndex - startIndex);
                int.TryParse(idString, out patientId);
            }
            
            await viewModel.LoadPatient(patientId);
        }
    }
}
