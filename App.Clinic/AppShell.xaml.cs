using App.Clinic.Views;

namespace App.Clinic;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes
        Routing.RegisterRoute(nameof(PatientPage), typeof(PatientPage));
        Routing.RegisterRoute(nameof(PatientManagementPage), typeof(PatientManagementPage));
    }
}
