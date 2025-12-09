using Library.Clinic.DTO;
using Library.Clinic.Models;
using Newtonsoft.Json;
using Library.Clinic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Clinic.Services
{
    public class PatientServiceProxy
    {
        private static object _lock = new object();
        private static PatientServiceProxy? instance;

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

        // THIS list is your REAL storage.
        // Never overwrite it during search.
        private List<PatientDTO> allPatients = new List<PatientDTO>();

        private PatientServiceProxy()
        {
            // Initialize with sample demo data
            allPatients = new List<PatientDTO> {
                new PatientDTO { Id = 1, Name = "John Doe", SSN = "123-45-6789", BirthDate = new DateTime(1980, 1, 15), Address = "123 Main St", Race = "Caucasian", Gender = "Male", Diagnoses = new List<string>{ "Hypertension" }, Prescriptions = new List<string>{ "Lisinopril" } },
                new PatientDTO { Id = 2, Name = "Jane Smith", SSN = "987-65-4321", BirthDate = new DateTime(1990, 5, 22), Address = "456 Oak Ave", Race = "Asian", Gender = "Female", Diagnoses = new List<string>{ "Asthma" }, Prescriptions = new List<string>{ "Albuterol" } },
                new PatientDTO { Id = 3, Name = "Bob Johnson", SSN = "456-78-9012", BirthDate = new DateTime(1975, 8, 10), Address = "789 Pine Rd", Race = "African American", Gender = "Male", Diagnoses = new List<string>{ "Diabetes Type 2" }, Prescriptions = new List<string>{ "Metformin" } }
            };
        }

        // Public-facing list (filtered or sorted)
        public List<PatientDTO> Patients { get; private set; } = new List<PatientDTO>();

        // Always use ALL patients for ID assignment
        public int LastKey
        {
            get
            {
                return allPatients.Count == 0 ? 0 : allPatients.Max(p => p.Id);
            }
        }

        // -----------------------------
        // SEARCH (FIXED)
        // -----------------------------
        public async Task<List<PatientDTO>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Patients = allPatients.ToList();
                return Patients;
            }

            try
            {
                var resultJson = await new WebRequestHandler()
                    .Post("/Patient/Search", new Query(query));

                var results = JsonConvert.DeserializeObject<List<PatientDTO>>(resultJson)
                              ?? new List<PatientDTO>();

                // Never overwrite allPatients with search results.
                Patients = results;
            }
            catch
            {
                // Local fallback search - but do NOT destroy allPatients
                Patients = allPatients
                    .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                p.SSN.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Patients;
        }

        // -----------------------------
        // ADD / UPDATE (FIXED)
        // -----------------------------
        public async Task<PatientDTO?> AddOrUpdatePatient(PatientDTO dto)
        {
            try
            {
                var payload = await new WebRequestHandler().Post("/patient", dto);
                var serverPatient = JsonConvert.DeserializeObject<PatientDTO>(payload);

                // If server actually gives a valid ID, use it.
                if (serverPatient != null && serverPatient.Id > 0)
                {
                    ApplyAddOrUpdate(serverPatient);
                    return serverPatient;
                }
            }
            catch
            {
                // server failed → ignore and fall back to local
            }

            // Local fallback:
            return AddOrUpdateLocal(dto);
        }

        private PatientDTO AddOrUpdateLocal(PatientDTO dto)
        {
            if (dto.Id == 0)
            {
                dto.Id = LastKey + 1;
            }

            ApplyAddOrUpdate(dto);

            return dto;
        }

        private void ApplyAddOrUpdate(PatientDTO dto)
        {
            var existing = allPatients.FirstOrDefault(x => x.Id == dto.Id);

            if (existing != null)
            {
                // Replace in master list
                var index = allPatients.IndexOf(existing);
                allPatients[index] = dto;
            }
            else
            {
                allPatients.Add(dto);
            }

            // Refresh visible list
            Patients = allPatients.ToList();
        }

        // -----------------------------
        // DELETE (FIXED)
        // -----------------------------
        public async void DeletePatient(int id)
        {
            var p = allPatients.FirstOrDefault(x => x.Id == id);

            if (p != null)
            {
                allPatients.Remove(p);
                Patients = allPatients.ToList();
            }

            try
            {
                await new WebRequestHandler().Delete($"/Patient/{id}");
            }
            catch
            {
                // local delete already happened
            }
        }
    }
}
