using Library.Clinic.DTO;
using Library.Clinic.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace App.Clinic.ViewModels;

[QueryProperty(nameof(PatientId), "patientId")]
public class PatientDetailsViewModel : INotifyPropertyChanged
{
    private bool _isNewPatient;
    private PatientDTO? _patient;
    private PatientViewModel? _patientViewModel;

    public int PatientId { get; set; }

    public PatientViewModel? Patient
    {
        get => _patientViewModel;
        set
        {
            if (_patientViewModel != value)
            {
                _patientViewModel = value;
                OnPropertyChanged(nameof(Patient));
            }
        }
    }

    public PatientDetailsViewModel()
    {
        SaveCommand = new Command(async () => await SavePatient(), CanSave);
        CancelCommand = new Command(async () => await Cancel());
    }

    public void Load()
    {
        System.Diagnostics.Debug.WriteLine($"Loading patient with ID: {PatientId}");
        
        if (PatientId > 0)
        {
            var dto = PatientServiceProxy.Current.Patients
                      .FirstOrDefault(x => x.Id == PatientId);
            
            if (dto != null)
            {
                Patient = new PatientViewModel(dto);
                _isNewPatient = false;
            }
            else
            {
                Patient = new PatientViewModel(new PatientDTO());
                _isNewPatient = true;
            }
        }
        else
        {
            Patient = new PatientViewModel(new PatientDTO());
            _isNewPatient = true;
        }
        
        // Listen to property changes to update CanSave
        if (Patient != null)
        {
            Patient.PropertyChanged += (sender, e) =>
            {
                ((Command)SaveCommand).ChangeCanExecute();
            };
        }
    }

    private bool CanSave()
    {
        var canSave = Patient?.Model != null && 
                     !string.IsNullOrWhiteSpace(Patient.Model.Name) && 
                     !string.IsNullOrWhiteSpace(Patient.Model.SSN) &&
                     Patient.Model.BirthDate != default;
        
        System.Diagnostics.Debug.WriteLine($"CanSave: {canSave}, Name: '{Patient?.Model?.Name}', SSN: '{Patient?.Model?.SSN}', BirthDate: {Patient?.Model?.BirthDate}");
        
        return canSave;
    }

    public Command SaveCommand { get; }
    public Command CancelCommand { get; }

    private async Task SavePatient()
    {
        try
        {
            if (Patient?.Model != null && CanSave())
            {
                System.Diagnostics.Debug.WriteLine($"Saving patient with ID={Patient.Model.Id}");
                
                var result = await PatientServiceProxy.Current.AddOrUpdatePatient(Patient.Model);
                if (result != null)
                {
                    await Shell.Current.GoToAsync("///PatientManagement");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to save patient. Please try again.", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Validation Error", "Please fill in all required fields (Name, SSN, and Birth Date).", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("///PatientManagement");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
