using Library.Clinic.DTO;
using Library.Clinic.Models;
using Library.Clinic.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace App.Clinic.ViewModels
{
    public class AppointmentViewModel : INotifyPropertyChanged
    {
        public AppointmentDTO? Model { get; set; }

        public int Id
        {
            get => Model?.Id ?? 0;
            set
            {
                if (Model != null && Model.Id != value)
                {
                    Model.Id = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Date
        {
            get => Model?.Date ?? DateTime.Today;
            set
            {
                if (Model != null && Model.Date != value)
                {
                    Model.Date = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime StartTime
        {
            get => Model?.StartTime ?? DateTime.Now;
            set
            {
                if (Model != null && Model.StartTime != value)
                {
                    Model.StartTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime EndTime
        {
            get => Model?.EndTime ?? DateTime.Now.AddHours(1);
            set
            {
                if (Model != null && Model.EndTime != value)
                {
                    Model.EndTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PatientId
        {
            get => Model?.PatientId ?? 0;
            set
            {
                if (Model != null && Model.PatientId != value)
                {
                    Model.PatientId = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PatientName));
                }
            }
        }

        public string? PatientName
        {
            get
            {
                if (Model != null && Model.PatientId > 0)
                {
                    if (string.IsNullOrEmpty(Model.PatientName))
                    {
                        var patient = PatientServiceProxy.Current.Patients.FirstOrDefault(p => p.Id == Model.PatientId);
                        Model.PatientName = patient?.Name;
                    }
                }
                return Model?.PatientName ?? string.Empty;
            }
        }

        public int PhysicianId
        {
            get => Model?.PhysicianId ?? 0;
            set
            {
                if (Model != null && Model.PhysicianId != value)
                {
                    Model.PhysicianId = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PhysicianName));
                }
            }
        }

        public string? PhysicianName
        {
            get
            {
                if (Model != null && Model.PhysicianId > 0)
                {
                    if (string.IsNullOrEmpty(Model.PhysicianName))
                    {
                        var physician = PhysicianServiceProxy.Current.Physicians.FirstOrDefault(p => p.Id == Model.PhysicianId);
                        Model.PhysicianName = physician?.Name;
                    }
                }
                return Model?.PhysicianName ?? string.Empty;
            }
        }

        public string? Reason
        {
            get => Model?.Reason ?? string.Empty;
            set
            {
                if (Model != null && Model.Reason != value)
                {
                    Model.Reason = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Notes
        {
            get => Model?.Notes ?? string.Empty;
            set
            {
                if (Model != null && Model.Notes != value)
                {
                    Model.Notes = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Room
        {
            get => Model?.Room ?? string.Empty;
            set
            {
                if (Model != null && Model.Room != value)
                {
                    Model.Room = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsToday
        {
            get => (Model?.Date.Date ?? DateTime.MinValue.Date) == DateTime.Today;
        }

        public ICommand DeleteCommand { get; private set; }
        public ICommand EditCommand { get; private set; }

        public AppointmentViewModel()
        {
            Model = new AppointmentDTO();
            DeleteCommand = new Command(async () => await DoDelete());
            EditCommand = new Command(async () => await DoEdit());
        }

        public AppointmentViewModel(AppointmentDTO appointment)
        {
            Model = appointment;
            DeleteCommand = new Command(async () => await DoDelete());
            EditCommand = new Command(async () => await DoEdit());
        }

        private async Task DoEdit()
        {
            var selectedAppointmentId = Model?.Id ?? 0;
            await Shell.Current.GoToAsync($"AppointmentDetails?appointmentId={selectedAppointmentId}");
        }

        private async Task DoDelete()
        {
            if (Model != null)
            {
                bool result = await Shell.Current.DisplayAlert(
                    "Confirm Delete",
                    $"Are you sure you want to delete the appointment for {PatientName}?",
                    "Delete",
                    "Cancel"
                );

                if (result)
                {
                    await AppointmentServiceProxy.Current.DeleteAppointment(Model.Id);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}