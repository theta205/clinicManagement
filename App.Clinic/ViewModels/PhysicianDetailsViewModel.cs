using Library.Clinic.DTO;
using Library.Clinic.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace App.Clinic.ViewModels;

public class PhysicianDetailsViewModel : INotifyPropertyChanged
{
    private PhysicianDTO? _physician;
    private bool _isNewPhysician;

    public PhysicianDetailsViewModel()
    {
        _physician = new PhysicianDTO();
        _isNewPhysician = true;
        
        SaveCommand = new Command(async () => await SavePhysician(), CanSave);
        CancelCommand = new Command(async () => await Cancel());
    }

    private bool CanSave()
    {
        return _physician != null && 
               !string.IsNullOrWhiteSpace(_physician.Name) && 
               !string.IsNullOrWhiteSpace(_physician.LicenseNumber) &&
               _physician.GraduationDate != default;
    }

    public int Id
    {
        get => _physician?.Id ?? 0;
        set
        {
            if (_physician != null && _physician.Id != value)
            {
                _physician.Id = value;
                OnPropertyChanged();
            }
        }
    }

    public string Name
    {
        get => _physician?.Name ?? string.Empty;
        set
        {
            if (_physician != null && _physician.Name != value)
            {
                _physician.Name = value;
                OnPropertyChanged();
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    public string LicenseNumber
    {
        get => _physician?.LicenseNumber ?? string.Empty;
        set
        {
            if (_physician != null && _physician.LicenseNumber != value)
            {
                _physician.LicenseNumber = value;
                OnPropertyChanged();
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    public DateTime GraduationDate
    {
        get => _physician?.GraduationDate ?? DateTime.Now;
        set
        {
            if (_physician != null && _physician.GraduationDate != value)
            {
                _physician.GraduationDate = value;
                OnPropertyChanged();
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    public string SpecializationsText
    {
        get => _physician != null ? string.Join(", ", _physician.Specializations) : string.Empty;
        set
        {
            if (_physician != null)
            {
                var newSpecializations = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                               .Select(s => s.Trim())
                                               .Where(s => !string.IsNullOrEmpty(s))
                                               .ToList();
                
                if (!_physician.Specializations.SequenceEqual(newSpecializations))
                {
                    _physician.Specializations = newSpecializations;
                    OnPropertyChanged();
                }
            }
        }
    }

    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }

    public async Task LoadPhysician(int physicianId)
    {
        if (physicianId > 0)
        {
            _physician = PhysicianServiceProxy.Current.Physicians.FirstOrDefault(p => p.Id == physicianId);
            _isNewPhysician = false;
        }
        else
        {
            _physician = new PhysicianDTO();
            _isNewPhysician = true;
        }

        OnPropertyChanged(nameof(Id));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(LicenseNumber));
        OnPropertyChanged(nameof(GraduationDate));
        OnPropertyChanged(nameof(SpecializationsText));
    }

    private async Task SavePhysician()
    {
        try
        {
            if (_physician != null && CanSave())
            {
                var result = await PhysicianServiceProxy.Current.AddOrUpdatePhysician(_physician);
                if (result != null)
                {
                    await Shell.Current.GoToAsync("///PhysicianManagement");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to save physician. Please try again.", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Validation Error", "Please fill in all required fields (Name, License Number, and Graduation Date).", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("///PhysicianManagement");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
