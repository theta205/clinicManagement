using Library.Clinic.DTO;
using Library.Clinic.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace App.Clinic.ViewModels
{
    public class PatientDetailsViewModel : INotifyPropertyChanged
    {
        private PatientDTO? _patient;
        private bool _isNewPatient;

        public PatientDetailsViewModel()
        {
            _patient = new PatientDTO();
            _isNewPatient = true;
            
            SaveCommand = new Command(async () => await SavePatient(), CanSave);
            CancelCommand = new Command(async () => await Cancel());
        }

        private bool CanSave()
        {
            return _patient != null && 
                   !string.IsNullOrWhiteSpace(_patient.Name) && 
                   !string.IsNullOrWhiteSpace(_patient.SSN) &&
                   _patient.BirthDate != default;
        }

        public int Id
        {
            get => _patient?.Id ?? 0;
            set
            {
                if (_patient != null && _patient.Id != value)
                {
                    _patient.Id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _patient?.Name ?? string.Empty;
            set
            {
                if (_patient != null && _patient.Name != value)
                {
                    _patient.Name = value;
                    OnPropertyChanged();
                    ((Command)SaveCommand).ChangeCanExecute();
                }
            }
        }

        public string SSN
        {
            get => _patient?.SSN ?? string.Empty;
            set
            {
                if (_patient != null && _patient.SSN != value)
                {
                    _patient.SSN = value;
                    OnPropertyChanged();
                    ((Command)SaveCommand).ChangeCanExecute();
                }
            }
        }

        public DateTime BirthDate
        {
            get => _patient?.BirthDate ?? DateTime.Now;
            set
            {
                if (_patient != null && _patient.BirthDate != value)
                {
                    _patient.BirthDate = value;
                    OnPropertyChanged();
                    ((Command)SaveCommand).ChangeCanExecute();
                }
            }
        }

        public string? Address
        {
            get => _patient?.Address;
            set
            {
                if (_patient != null && _patient.Address != value)
                {
                    _patient.Address = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public async Task LoadPatient(int patientId)
        {
            if (patientId > 0)
            {
                _patient = PatientServiceProxy.Current.Patients.FirstOrDefault(p => p.Id == patientId);
                _isNewPatient = false;
            }
            else
            {
                _patient = new PatientDTO();
                _isNewPatient = true;
            }

            OnPropertyChanged(nameof(Id));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(SSN));
            OnPropertyChanged(nameof(BirthDate));
            OnPropertyChanged(nameof(Address));
        }

        private async Task SavePatient()
        {
            try
            {
                if (_patient != null && CanSave())
                {
                    var result = await PatientServiceProxy.Current.AddOrUpdatePatient(_patient);
                    if (result != null)
                    {
                        await Shell.Current.GoToAsync("/PatientManagement");
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
            await Shell.Current.GoToAsync("/PatientManagement");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
