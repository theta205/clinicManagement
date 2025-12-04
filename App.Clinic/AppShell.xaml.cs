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
        Routing.RegisterRoute("PatientDetails", typeof(PatientDetailsPage));
        
        Routing.RegisterRoute(nameof(PhysicianManagementPage), typeof(PhysicianManagementPage));
        Routing.RegisterRoute("PhysicianDetails", typeof(PhysicianDetailsPage));
        
        Routing.RegisterRoute(nameof(AppointmentPage), typeof(AppointmentPage));
    }
}
