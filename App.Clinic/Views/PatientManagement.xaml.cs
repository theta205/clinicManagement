using App.Clinic.ViewModels;

namespace App.Clinic.Views;

public partial class PatientManagementPage : ContentPage
{
    public PatientManagementPage()
    {
        InitializeComponent();
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is PatientViewModel patient)
        {
            await Shell.Current.GoToAsync($"//PatientDetails?patientId={patient.Id}");
        }
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is PatientViewModel patient)
        {
            var viewModel = BindingContext as PatientManagementViewModel;
            if (viewModel != null)
            {
                viewModel.SelectedPatient = patient;
                viewModel.Delete();
            }
        }
    }
}
