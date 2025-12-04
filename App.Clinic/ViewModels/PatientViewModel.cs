using Library.Clinic.DTO;
using Library.Clinic.Models;
using Library.Clinic.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace App.Clinic.ViewModels
{
    public class PatientViewModel : INotifyPropertyChanged
    {
        public PatientDTO? Model { get; set; }
        public ICommand? DeleteCommand { get; set; }
        public ICommand? EditCommand { get; set; }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int Id
        {
            get
            {
                if(Model == null)
                {
                    return -1;
                }

                return Model.Id;
            }

            set
            {
                if(Model != null && Model.Id != value) {
                    Model.Id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => Model?.Name ?? string.Empty;
            set
            {
                if(Model != null)
                {
                    Model.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SSN
        {
            get => Model?.SSN ?? string.Empty;
            set
            {
                if(Model != null)
                {
                    Model.SSN = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime BirthDate
        {
            get => Model?.BirthDate ?? DateTime.Now;
            set
            {
                if(Model != null)
                {
                    Model.BirthDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Address
        {
            get => Model?.Address;
            set
            {
                if(Model != null)
                {
                    Model.Address = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Race
        {
            get => Model?.Race ?? string.Empty;
            set
            {
                if(Model != null)
                {
                    Model.Race = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Gender
        {
            get => Model?.Gender ?? string.Empty;
            set
            {
                if(Model != null)
                {
                    Model.Gender = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DiagnosesText
        {
            get => Model != null ? string.Join(", ", Model.Diagnoses) : string.Empty;
            set
            {
                if (Model != null)
                {
                    var newDiagnoses = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(s => s.Trim())
                                           .Where(s => !string.IsNullOrEmpty(s))
                                           .ToList();
                    
                    if (!Model.Diagnoses.SequenceEqual(newDiagnoses))
                    {
                        Model.Diagnoses = newDiagnoses;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public string PrescriptionsText
        {
            get => Model != null ? string.Join(", ", Model.Prescriptions) : string.Empty;
            set
            {
                if (Model != null)
                {
                    var newPrescriptions = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                               .Select(s => s.Trim())
                                               .Where(s => !string.IsNullOrEmpty(s))
                                               .ToList();
                    
                    if (!Model.Prescriptions.SequenceEqual(newPrescriptions))
                    {
                        Model.Prescriptions = newPrescriptions;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public void SetupCommands()
        {
            DeleteCommand = new Command(DoDelete);
            EditCommand = new Command((p) => DoEdit(p as PatientViewModel));
        }

        private void DoDelete()
        {
            if (Id > 0)
            {
                PatientServiceProxy.Current.DeletePatient(Id);
                Shell.Current.GoToAsync("///PatientManagement");
            }
        }

        private void DoEdit(PatientViewModel? pvm)
        {
            if (pvm == null)
            {
                return;
            }
            var selectedPatientId = pvm?.Id ?? 0;
            Shell.Current.GoToAsync($"/PatientDetails?patientId={selectedPatientId}");
        }

        public PatientViewModel()
        {
            Model = new PatientDTO();
            SetupCommands();
        }

        public PatientViewModel(PatientDTO? _model)
        {
            Model = _model;
            SetupCommands();
        }

        public async void ExecuteAdd()
        {
            if (Model != null)
            {
                await PatientServiceProxy
                .Current
                .AddOrUpdatePatient(Model);
            }

            await Shell.Current.GoToAsync("///PatientManagement");
        }
    }
}