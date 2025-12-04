using App.Clinic.ViewModels;

namespace App.Clinic.Views;

public partial class PatientDetailsPage : ContentPage
{
    public PatientDetailsPage()
    {
        InitializeComponent();
        BindingContext = new PatientDetailsViewModel();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        (BindingContext as PatientDetailsViewModel)?.Load();
    }
}
