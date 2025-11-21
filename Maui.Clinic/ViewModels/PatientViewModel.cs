using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Library.Clinic.Models;
using Microsoft.Maui.Controls;

namespace Maui.Clinic.ViewModels;

public class PatientViewModel : INotifyPropertyChanged
{
    private static readonly List<Patient> _patients = new();
    private static int _nextId = 1;

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
    
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
            SaveCommand.ChangeCanExecute();
        }
    }
    
    private DateTime _birthDate = DateTime.Today.AddYears(-30);
    public DateTime BirthDate
    {
        get => _birthDate;
        set => SetProperty(ref _birthDate, value);
    }
    
    private string _gender = "Male";
    public string Gender
    {
        get => _gender;
        set => SetProperty(ref _gender, value);
    }
    
    private string _race = "White";
    public string Race
    {
        get => _race;
        set => SetProperty(ref _race, value);
    }
    
    private string? _address;
    public string? Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }
    
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }
    
    public ObservableCollection<Patient> Patients { get; } = new();
    
    public List<string> Genders { get; } = new() { "Male", "Female", "Other" };
    public List<string> Races { get; } = new() { "White", "Black or African American", "Asian", "Native American", "Other" };
    
    public string Title => Id == 0 ? "New Patient" : "Edit Patient";
    
    public Command SaveCommand { get; }
    public Command AddNewPatientCommand { get; }
    public Command<Patient> EditPatientCommand { get; }
    public Command<Patient> DeletePatientCommand { get; }
    public Command ToggleEditCommand { get; }
    public Command CancelEditCommand { get; }

    public PatientViewModel()
    {
        SaveCommand = new Command(async () => await Save(), CanSave);
        AddNewPatientCommand = new Command(async () => await AddNewPatient());
        EditPatientCommand = new Command<Patient>(async (p) => await EditPatient(p));
        DeletePatientCommand = new Command<Patient>(async (p) => await DeletePatient(p));
        ToggleEditCommand = new Command(ToggleEdit);
        CancelEditCommand = new Command(CancelEdit);

        // Add some sample data if empty
        if (_patients.Count == 0)
        {
            _patients.Add(new Patient 
            { 
                Id = _nextId++,
                Name = "John Doe",
                BirthDate = new DateTime(1980, 1, 1),
                Gender = "Male",
                Race = "White"
            });
        }
        LoadPatients();
    }
        
    public async Task LoadPatients()
    {
        if (IsBusy) return;
        
        IsBusy = true;
        
        try
        {
            await Task.Run(() =>
            {
                var patients = _patients.OrderBy(p => p.Name).ToList();
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Patients.Clear();
                    foreach (var patient in patients)
                    {
                        Patients.Add(patient);
                    }
                });
            });
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load patients: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private void ToggleEdit()
    {
        IsEditing = !IsEditing;
    }

    private void CancelEdit()
    {
        IsEditing = false;
    }

    private async Task AddNewPatient()
    {
        Id = 0;
        Name = string.Empty;
        BirthDate = DateTime.Today.AddYears(-30);
        Gender = "Male";
        Race = "White";
        Address = null;
        IsEditing = true;
    }
    
    private async Task EditPatient(Patient patient)
    {
        if (patient == null) return;
        
        Id = patient.Id;
        Name = patient.Name;
        BirthDate = patient.BirthDate;
        Gender = patient.Gender;
        Race = patient.Race;
        Address = patient.Address;
        IsEditing = true;
    }
    
    private async Task DeletePatient(Patient patient)
    {
        if (patient == null) return;
        
        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Delete Patient",
            $"Are you sure you want to delete {patient.Name}?",
            "Yes", "No");
            
        if (confirm)
        {
            var patientToRemove = _patients.FirstOrDefault(p => p.Id == patient.Id);
            if (patientToRemove != null)
            {
                _patients.Remove(patientToRemove);
                await LoadPatients();
            }
        }
    }
    
    private async Task Save()
{
    if (IsBusy || !CanSave()) return;
    
    IsBusy = true;
    
    try
    {
        var patient = new Patient
        {
            Id = Id == 0 ? _nextId++ : Id,
            Name = Name,
            BirthDate = BirthDate,
            Gender = Gender,
            Race = Race,
            Address = Address
        };
        
        if (Id == 0)
        {
            _patients.Add(patient);
            await Application.Current.MainPage.DisplayAlert("Success", "Patient added successfully", "OK");
        }
        else
        {
            var index = _patients.FindIndex(p => p.Id == Id);
            if (index >= 0)
            {
                _patients[index] = patient;
                await Application.Current.MainPage.DisplayAlert("Success", "Patient updated successfully", "OK");
            }
        }
        
        await LoadPatients();
        IsEditing = false;
    }
    catch (Exception ex)
    {
        await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
    }
    finally
    {
        IsBusy = false;
    }
}
    
    private bool CanSave() => 
        !string.IsNullOrWhiteSpace(Name);

    protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action? onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}