using Library.Clinic.DTO;
using Library.Clinic.Services;
using System.Windows.Input;

namespace App.Clinic.ViewModels;

public class PhysicianViewModel
{
    public PhysicianDTO? Model { get; set; }
    
    public int Id
    {
        get => Model?.Id ?? 0;
        set
        {
            if (Model != null && Model.Id != value)
            {
                Model.Id = value;
            }
        }
    }

    public string Name
    {
        get => Model?.Name ?? string.Empty;
        set
        {
            if (Model != null && Model.Name != value)
            {
                Model.Name = value;
            }
        }
    }

    public string LicenseNumber
    {
        get => Model?.LicenseNumber ?? string.Empty;
        set
        {
            if (Model != null && Model.LicenseNumber != value)
            {
                Model.LicenseNumber = value;
            }
        }
    }

    public DateTime GraduationDate
    {
        get => Model?.GraduationDate ?? DateTime.Now;
        set
        {
            if (Model != null && Model.GraduationDate != value)
            {
                Model.GraduationDate = value;
            }
        }
    }

    public List<string> Specializations
    {
        get => Model?.Specializations ?? new List<string>();
        set
        {
            if (Model != null && Model.Specializations != value)
            {
                Model.Specializations = value;
            }
        }
    }

    public string SpecializationsText
    {
        get => Model != null ? string.Join(", ", Model.Specializations) : string.Empty;
        set
        {
            if (Model != null)
            {
                Model.Specializations = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(s => s.Trim())
                                           .Where(s => !string.IsNullOrEmpty(s))
                                           .ToList();
            }
        }
    }

    public ICommand DeleteCommand { get; private set; }
    public ICommand EditCommand { get; private set; }

    public PhysicianViewModel()
    {
        Model = new PhysicianDTO();
        SetupCommands();
    }

    public PhysicianViewModel(PhysicianDTO? _model)
    {
        Model = _model ?? new PhysicianDTO();
        SetupCommands();
    }

    private void SetupCommands()
    {
        DeleteCommand = new Command(DoDelete);
        EditCommand = new Command((p) => DoEdit(p as PhysicianViewModel));
    }

    private void DoDelete()
    {
        if (Id > 0)
        {
            PhysicianServiceProxy.Current.DeletePhysician(Id);
            Shell.Current.GoToAsync("///PhysicianManagement");
        }
    }

    private void DoEdit(PhysicianViewModel? pvm)
    {
        if (pvm == null)
        {
            return;
        }
        var selectedPhysicianId = pvm?.Id ?? 0;
        Shell.Current.GoToAsync($"/PhysicianDetails?physicianId={selectedPhysicianId}");
    }

    public async void ExecuteAdd()
    {
        if (Model != null)
        {
            await PhysicianServiceProxy
            .Current
            .AddOrUpdatePhysician(Model);
        }

        await Shell.Current.GoToAsync("///PhysicianManagement");
    }
}
