using Maui.Clinic.ViewModels;

namespace Maui.Clinic.Views;

public partial class PatientView : ContentPage
{
    public PatientView(PatientViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PatientViewModel viewModel)
        {
            viewModel.LoadPatients();
        }
    }
}