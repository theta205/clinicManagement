using Library.Clinic.DTO;
using Library.Clinic.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace App.Clinic.ViewModels;

[QueryProperty(nameof(AppointmentId), "appointmentId")]
public class AppointmentDetailsViewModel : INotifyPropertyChanged
{
    private AppointmentDTO? _appointment;
    private bool _isNewAppointment;

    public int AppointmentId { get; set; }

    public AppointmentDetailsViewModel()
    {
        _appointment = new AppointmentDTO();
        _isNewAppointment = true;
        
        System.Diagnostics.Debug.WriteLine("[AppointmentDetails] ViewModel created, setting up commands");
        SaveCommand = new Command(async () => await SaveAppointment(), CanSave);
        CancelCommand = new Command(async () => await Cancel());
        AddTreatmentCommand = new Command(AddTreatment);
        RemoveTreatmentCommand = new Command<TreatmentDTO>(RemoveTreatment);
    }

    private bool CanSave()
    {
        var canSave = _appointment != null && 
               _appointment.PatientId > 0 && 
               _appointment.PhysicianId > 0 &&
               _appointment.Date != default &&
               _appointment.StartTime != default &&
               _appointment.EndTime != default &&
               _appointment.StartTime < _appointment.EndTime;
        
        System.Diagnostics.Debug.WriteLine($"[AppointmentDetails] CanSave: {canSave}, PatientId: {_appointment?.PatientId}, PhysicianId: {_appointment?.PhysicianId}, Date: {_appointment?.Date}");
        
        return canSave;
    }

    public int Id
    {
        get => _appointment?.Id ?? 0;
        set
        {
            if (_appointment != null && _appointment.Id != value)
            {
                _appointment.Id = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime Date
    {
        get => _appointment?.Date ?? DateTime.Today;
        set
        {
            if (_appointment != null && _appointment.Date != value)
            {
                _appointment.Date = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MinDate));
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    public DateTime MinDate => DateTime.Today;

    public DateTime StartTime
    {
        get => _appointment?.StartTime ?? DateTime.Now;
        set
        {
            if (_appointment != null && _appointment.StartTime != value)
            {
                _appointment.StartTime = value;
                OnPropertyChanged();
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    public DateTime EndTime
    {
        get => _appointment?.EndTime ?? DateTime.Now.AddHours(1);
        set
        {
            if (_appointment != null && _appointment.EndTime != value)
            {
                _appointment.EndTime = value;
                OnPropertyChanged();
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    public int PatientId
    {
        get => _appointment?.PatientId ?? 0;
        set
        {
            if (_appointment != null && _appointment.PatientId != value)
            {
                _appointment.PatientId = value;
                OnPropertyChanged();
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    public string? PatientName
    {
        get => _appointment?.PatientName;
        set
        {
            if (_appointment != null && _appointment.PatientName != value)
            {
                _appointment.PatientName = value;
                OnPropertyChanged();
            }
        }
    }

    public int PhysicianId
    {
        get => _appointment?.PhysicianId ?? 0;
        set
        {
            if (_appointment != null && _appointment.PhysicianId != value)
            {
                _appointment.PhysicianId = value;
                OnPropertyChanged();
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    public string? PhysicianName
    {
        get => _appointment?.PhysicianName;
        set
        {
            if (_appointment != null && _appointment.PhysicianName != value)
            {
                _appointment.PhysicianName = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Reason
    {
        get => _appointment?.Reason ?? string.Empty;
        set
        {
            if (_appointment != null && _appointment.Reason != value)
            {
                _appointment.Reason = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Notes
    {
        get => _appointment?.Notes ?? string.Empty;
        set
        {
            if (_appointment != null && _appointment.Notes != value)
            {
                _appointment.Notes = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Room
    {
        get => _appointment?.Room ?? string.Empty;
        set
        {
            if (_appointment != null && _appointment.Room != value)
            {
                _appointment.Room = value;
                OnPropertyChanged();
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    public ObservableCollection<TreatmentDTO> Treatments
    {
        get => new ObservableCollection<TreatmentDTO>(_appointment?.Treatments ?? new List<TreatmentDTO>());
    }

    private string _newTreatmentName = string.Empty;
    public string NewTreatmentName
    {
        get => _newTreatmentName;
        set
        {
            if (_newTreatmentName != value)
            {
                _newTreatmentName = value;
                OnPropertyChanged();
            }
        }
    }

    private string _newTreatmentCostText = string.Empty;
    public string NewTreatmentCostText
    {
        get => _newTreatmentCostText;
        set
        {
            if (_newTreatmentCostText != value)
            {
                _newTreatmentCostText = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal TotalCost
    {
        get => (decimal)(_appointment?.Treatments?.Sum(t => t.Cost) ?? 0m);
    }

    public ICommand AddTreatmentCommand { get; private set; }
    public ICommand RemoveTreatmentCommand { get; private set; }

    public List<PatientDTO> Patients => PatientServiceProxy.Current.Patients;

    public List<PhysicianDTO> Physicians => PhysicianServiceProxy.Current.Physicians;

    private PatientDTO? _selectedPatient;
    public PatientDTO? SelectedPatient
    {
        get => _selectedPatient;
        set
        {
            if (_selectedPatient != value)
            {
                _selectedPatient = value;
                if (_appointment != null && value != null)
                {
                    _appointment.PatientId = value.Id;
                    _appointment.PatientName = value.Name;
                    OnPropertyChanged(nameof(PatientId));
                    OnPropertyChanged(nameof(PatientName));
                    ((Command)SaveCommand).ChangeCanExecute();
                }
            }
        }
    }

    private PhysicianDTO? _selectedPhysician;
    public PhysicianDTO? SelectedPhysician
    {
        get => _selectedPhysician;
        set
        {
            if (_selectedPhysician != value)
            {
                _selectedPhysician = value;
                if (_appointment != null && value != null)
                {
                    _appointment.PhysicianId = value.Id;
                    _appointment.PhysicianName = value.Name;
                    OnPropertyChanged(nameof(PhysicianId));
                    OnPropertyChanged(nameof(PhysicianName));
                    ((Command)SaveCommand).ChangeCanExecute();
                }
            }
        }
    }

    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }

    private async Task LoadAppointment(int appointmentId)
    {
        if (appointmentId > 0)
        {
            _appointment = AppointmentServiceProxy.Current.Appointments
                            .FirstOrDefault(a => a.Id == appointmentId);
            _isNewAppointment = false;
        }
        else
        {
            _appointment = new AppointmentDTO
            {
                Date = DateTime.Today,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            _isNewAppointment = true;
        }

        // Notify all properties changed
        OnPropertyChanged(nameof(Id));
        OnPropertyChanged(nameof(Date));
        OnPropertyChanged(nameof(StartTime));
        OnPropertyChanged(nameof(EndTime));
        OnPropertyChanged(nameof(PatientId));
        OnPropertyChanged(nameof(PatientName));
        OnPropertyChanged(nameof(PhysicianId));
        OnPropertyChanged(nameof(PhysicianName));
        OnPropertyChanged(nameof(Reason));
        OnPropertyChanged(nameof(Notes));
        OnPropertyChanged(nameof(Room));
        OnPropertyChanged(nameof(Treatments));
        OnPropertyChanged(nameof(TotalCost));

        // 🔥 This is the required fix:
        ((Command)SaveCommand).ChangeCanExecute();
    }

    // Used by Shell navigation + QueryProperty
    public async Task Load()
    {
        await LoadAppointment(AppointmentId);
    }

    private async Task SaveAppointment()
    {
        try
        {
            if (_appointment != null && CanSave())
            {
                // Update patient and physician names based on selected IDs
                var selectedPatient = PatientServiceProxy.Current.Patients.FirstOrDefault(p => p.Id == _appointment.PatientId);
                var selectedPhysician = PhysicianServiceProxy.Current.Physicians.FirstOrDefault(p => p.Id == _appointment.PhysicianId);
                
                if (selectedPatient != null)
                {
                    _appointment.PatientName = selectedPatient.Name;
                }
                
                if (selectedPhysician != null)
                {
                    _appointment.PhysicianName = selectedPhysician.Name;
                }

                // Client-side room conflict validation: no overlapping
                // appointments in the same room at the same time.
                if (!string.IsNullOrWhiteSpace(_appointment.Room))
                {
                    var sameRoomAppointments = AppointmentServiceProxy.Current.Appointments
                        .Where(a => a.Room != null &&
                                    a.Room.Equals(_appointment.Room, StringComparison.OrdinalIgnoreCase) &&
                                    a.Id != _appointment.Id &&
                                    a.Date.Date == _appointment.Date.Date);

                    bool hasOverlap = sameRoomAppointments.Any(a =>
                        _appointment.StartTime < a.EndTime &&
                        _appointment.EndTime > a.StartTime);

                    if (hasOverlap)
                    {
                        await Shell.Current.DisplayAlert(
                            "Scheduling Conflict",
                            $"Another appointment is already scheduled in room '{_appointment.Room}' during this time.",
                            "OK");
                        return;
                    }
                }

                var result = await AppointmentServiceProxy.Current.AddOrUpdateAppointment(_appointment);
                if (result != null)
                {
                    System.Diagnostics.Debug.WriteLine("Navigating to AppointmentManagement...");
                    await Shell.Current.GoToAsync("///AppointmentPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to save appointment. Please try again.", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Validation Error", "Please fill in all required fields (Patient, Physician, Date, Start Time, and End Time).", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async Task Cancel()
    {
        System.Diagnostics.Debug.WriteLine("Navigating to AppointmentManagement...");
        await Shell.Current.GoToAsync("///AppointmentPage");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void AddTreatment()
    {
        if (_appointment == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(NewTreatmentName))
        {
            return;
        }

        if (!decimal.TryParse(NewTreatmentCostText, out var cost) || cost < 0)
        {
            return;
        }

        _appointment.Treatments ??= new List<TreatmentDTO>();
        _appointment.Treatments.Add(new TreatmentDTO
        {
            Name = NewTreatmentName.Trim(),
            Cost = cost
        });

        NewTreatmentName = string.Empty;
        NewTreatmentCostText = string.Empty;

        OnPropertyChanged(nameof(Treatments));
        OnPropertyChanged(nameof(TotalCost));
    }

    private void RemoveTreatment(TreatmentDTO? treatment)
    {
        if (_appointment?.Treatments == null || treatment == null)
        {
            return;
        }

        if (_appointment.Treatments.Remove(treatment))
        {
            OnPropertyChanged(nameof(Treatments));
            OnPropertyChanged(nameof(TotalCost));
        }
    }
}
