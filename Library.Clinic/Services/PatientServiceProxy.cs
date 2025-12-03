using Library.Clinic.DTO;
using Library.Clinic.Models;
using Newtonsoft.Json;
using Library.Clinic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Library.Clinic.Services
{
    public class PatientServiceProxy
    {
        private static object _lock = new object();
        public static PatientServiceProxy Current
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new PatientServiceProxy();
                    }
                }
                return instance;
            }
        }

        private static PatientServiceProxy? instance;
        private PatientServiceProxy()
        {
            instance = null;
            
            // Initialize with sample data to avoid blocking
            Patients = new List<PatientDTO> {
                new PatientDTO { Id = 1, Name = "John Doe", SSN = "123-45-6789", BirthDate = new DateTime(1980, 1, 15), Address = "123 Main St, Anytown, USA" },
                new PatientDTO { Id = 2, Name = "Jane Smith", SSN = "987-65-4321", BirthDate = new DateTime(1990, 5, 22), Address = "456 Oak Ave, Somewhere, USA" },
                new PatientDTO { Id = 3, Name = "Bob Johnson", SSN = "456-78-9012", BirthDate = new DateTime(1975, 8, 10), Address = "789 Pine Rd, Nowhere, USA" }
            };
            
            // Try to load data asynchronously without blocking
            _ = LoadPatientsAsync();
        }

        private async Task LoadPatientsAsync()
        {
            try
            {
                var patientsData = await new WebRequestHandler().Get("/Patient");
                var loadedPatients = JsonConvert.DeserializeObject<List<PatientDTO>>(patientsData) ?? new List<PatientDTO>();
                
                // Update the patients list on the main thread if needed
                Patients = loadedPatients;
            }
            catch (Exception)
            {
                // If web request fails, keep the empty list
                Patients = new List<PatientDTO>();
            }
        }
        public int LastKey
        {
            get
            {
                if (Patients.Any())
                {
                    return Patients.Select(x => x.Id).Max();
                }
                return 0;
            }
        }

        private List<PatientDTO> patients = new List<PatientDTO>();
        public List<PatientDTO> Patients {
            get {
                return patients;
            }

            private set
            {
                if (patients != value)
                {
                    patients = value;
                }
            }
        }

        public async Task<List<PatientDTO>> Search(string query) {
            try
            {
                var patientsPayload = await new WebRequestHandler()
                    .Post("/Patient/Search", new Query(query));

                Patients = JsonConvert.DeserializeObject<List<PatientDTO>>(patientsPayload)
                    ?? new List<PatientDTO>();
            }
            catch (Exception)
            {
                // Fallback to local search if web service fails
                if (string.IsNullOrWhiteSpace(query))
                {
                    // If query is empty, load all patients
                    Patients = Patients.ToList(); // Reset to full list
                }
                else
                {
                    // Filter locally
                    Patients = Patients
                        .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                   p.SSN.Contains(query, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            return Patients;
        }

        public async Task<PatientDTO?> AddOrUpdatePatient(PatientDTO patient)
        {
            try
            {
                // Try web service first
                var payload = await new WebRequestHandler().Post("/patient", patient);
                var newPatient = JsonConvert.DeserializeObject<PatientDTO>(payload);
                
                if(newPatient != null && newPatient.Id > 0 && patient.Id == 0)
                {
                    //new patient to be added to the list
                    Patients.Add(newPatient);
                } else if(newPatient != null && patient != null && patient.Id > 0 && patient.Id == newPatient.Id)
                {
                    //edit, exchange the object in the list
                    var currentPatient = Patients.FirstOrDefault(p => p.Id == newPatient.Id);
                    var index = Patients.Count;
                    if (currentPatient != null)
                    {
                        index = Patients.IndexOf(currentPatient);
                        Patients.RemoveAt(index);
                    }
                    Patients.Insert(index, newPatient);
                }
                
                return newPatient;
            }
            catch (Exception)
            {
                // Fallback to local storage if web service fails
                return AddOrUpdatePatientLocal(patient);
            }
        }

        private PatientDTO? AddOrUpdatePatientLocal(PatientDTO patient)
        {
            if(patient.Id == 0)
            {
                // New patient - assign ID and add to list
                patient.Id = LastKey + 1;
                Patients.Add(patient);
            }
            else
            {
                // Existing patient - update in list
                var existingPatient = Patients.FirstOrDefault(p => p.Id == patient.Id);
                if (existingPatient != null)
                {
                    var index = Patients.IndexOf(existingPatient);
                    Patients.RemoveAt(index);
                    Patients.Insert(index, patient);
                }
                else
                {
                    // Patient not found, add it
                    Patients.Add(patient);
                }
            }
            
            return patient;
        }

        public async void DeletePatient(int id) {
            var patientToRemove = Patients.FirstOrDefault(p => p.Id == id);

            if (patientToRemove != null)
            {
                Patients.Remove(patientToRemove);

                try
                {
                    // Try web service delete
                    await new WebRequestHandler().Delete($"/Patient/{id}");
                }
                catch (Exception)
                {
                    // Fallback to local only - patient already removed from list
                    // No action needed as we already removed from local list
                }
            }
        }
    }
}