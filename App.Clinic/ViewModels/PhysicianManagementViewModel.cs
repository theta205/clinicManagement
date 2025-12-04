using Library.Clinic.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace App.Clinic.ViewModels;

public enum PhysicianSortChoiceEnum
{
    NameAscending,
    NameDescending
}

public class PhysicianManagementViewModel: INotifyPropertyChanged
{
    public PhysicianManagementViewModel() {
        SortChoices = new List<PhysicianSortChoiceEnum>
        {
           PhysicianSortChoiceEnum.NameAscending
          , PhysicianSortChoiceEnum.NameDescending
        };

        SortChoice = PhysicianSortChoiceEnum.NameAscending;
        
        AddCommand = new Command(DoAdd);
        EditCommand = new Command(DoEdit);
        SearchCommand = new Command(Search);
        DeleteCommand = new Command(DoDelete);
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public List<PhysicianSortChoiceEnum> SortChoices { get; set; }

    private PhysicianSortChoiceEnum sortChoice;
    public PhysicianSortChoiceEnum SortChoice { 
        get
        {
            return sortChoice;
        }
        set
        {
            sortChoice = value;
            NotifyPropertyChanged(nameof(SortChoice));
            NotifyPropertyChanged(nameof(Physicians));
        }
    }

    private string query = string.Empty;
    public string Query { 
        get 
        { 
            return query; 
        } 
        set 
        { 
            query = value; 
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(Physicians));
        } 
    }

    public ObservableCollection<PhysicianViewModel> Physicians
    {
        get
        {
            var retVal = new ObservableCollection<PhysicianViewModel>(
                PhysicianServiceProxy
                .Current
                .Physicians
                .Where(p=>p != null)
                .Where(p => p.Name.ToUpper().Contains(Query?.ToUpper() ?? string.Empty))
                .Select(p => new PhysicianViewModel(p))
                );

            if(SortChoice == PhysicianSortChoiceEnum.NameAscending)
            {
                return
                    new ObservableCollection<PhysicianViewModel>(retVal.OrderBy(p => p.Name));
            } else
            {
                return
                    new ObservableCollection<PhysicianViewModel>(retVal.OrderByDescending(p => p.Name));
            }
        }
    }

    private PhysicianViewModel? selectedPhysician;
    public PhysicianViewModel? SelectedPhysician
    {
        get => selectedPhysician;
        set
        {
            selectedPhysician = value;
            NotifyPropertyChanged();
        }
    }

    public ICommand AddCommand { get; private set; }
    public ICommand EditCommand { get; private set; }
    public ICommand SearchCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

    public void Delete()
    {
        if(SelectedPhysician == null)
        {
            return;
        }
        PhysicianServiceProxy.Current.DeletePhysician(SelectedPhysician.Id);

        Refresh();
    }

    public void Refresh()
    {
        NotifyPropertyChanged(nameof(Physicians));
    }

    public async void Search()
    {
        if (Query != null)
        {
            Task.Run(async () => await PhysicianServiceProxy.Current.Search(Query));
        }
        Refresh();
    }

    private async void DoAdd()
    {
        try
        {
            await Shell.Current.GoToAsync("/PhysicianDetails");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
        }
    }

    private async void DoEdit()
    {
        if (SelectedPhysician != null)
        {
            await Shell.Current.GoToAsync($"/PhysicianDetails?physicianId={SelectedPhysician.Id}");
        }
    }

    private void DoDelete()
    {
        Delete();
    }
}
