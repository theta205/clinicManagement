using Library.Clinic.DTO;
using Library.Clinic.Models;
using Library.Clinic.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace App.Clinic.ViewModels
{
    public class AppointmentManagementViewModel : INotifyPropertyChanged
    {
        private AppointmentServiceProxy _appSvc = AppointmentServiceProxy.Current;

        public ObservableCollection<AppointmentViewModel> Appointments
        {
            get
            {
                return new ObservableCollection<AppointmentViewModel>(
                    _appSvc.Appointments.Select(a => new AppointmentViewModel(a)));
            }
        }

        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        private AppointmentViewModel? _selectedAppointment;
        public AppointmentViewModel? SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                _selectedAppointment = value;
                OnPropertyChanged(nameof(SelectedAppointment));
            }
        }

        public AppointmentManagementViewModel()
        {
            AddCommand = new Command(async () => await DoAdd());
            EditCommand = new Command(async () => await DoEdit());
            DeleteCommand = new Command(async () => await DoDelete());
        }

        private async Task DoAdd()
        {
            try
            {
                await Shell.Current.GoToAsync("AppointmentDetails");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        private async Task DoEdit()
        {
            if (SelectedAppointment != null)
            {
                await Shell.Current.GoToAsync($"AppointmentDetails?appointmentId={SelectedAppointment.Id}");
            }
        }

        private async Task DoDelete()
        {
            if (SelectedAppointment != null)
            {
                bool result = await Shell.Current.DisplayAlert(
                    "Confirm Delete",
                    $"Are you sure you want to delete the appointment for {SelectedAppointment.PatientName}?",
                    "Delete",
                    "Cancel"
                );

                if (result)
                {
                    await _appSvc.DeleteAppointment(SelectedAppointment.Id);
                    Refresh();
                }
            }
        }

        public void Refresh()
        {
            _appSvc.Refresh();
            OnPropertyChanged(nameof(Appointments));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}