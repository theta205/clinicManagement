using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Clinic.Models;
using Newtonsoft.Json;

namespace Library.Clinic.Services
{
    public class Filebase
    {
        private readonly string _root;
        private readonly string _patientRoot;
        private static Filebase? _instance;

        public static Filebase Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Filebase();
                }
                return _instance;
            }
        }

        private Filebase()
        {
            // Use a temp-like folder that works on macOS as well.
            _root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClinicData");
            _patientRoot = Path.Combine(_root, "Patients");

            if (!Directory.Exists(_patientRoot))
            {
                Directory.CreateDirectory(_patientRoot);
            }
        }

        public int LastPatientKey
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

        public Patient AddOrUpdate(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }

            // Assign a new Id if one doesn't already exist
            if (patient.Id <= 0)
            {
                patient.Id = LastPatientKey + 1;
            }

            var path = Path.Combine(_patientRoot, $"{patient.Id}.json");

            // Overwrite existing file if present
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var json = JsonConvert.SerializeObject(patient, Formatting.Indented);
            File.WriteAllText(path, json);

            return patient;
        }

        public List<Patient> Patients
        {
            get
            {
                var dir = new DirectoryInfo(_patientRoot);
                var patients = new List<Patient>();

                if (!dir.Exists)
                {
                    return patients;
                }

                foreach (var patientFile in dir.GetFiles("*.json"))
                {
                    try
                    {
                        var contents = File.ReadAllText(patientFile.FullName);
                        var patient = JsonConvert.DeserializeObject<Patient>(contents);
                        if (patient != null)
                        {
                            patients.Add(patient);
                        }
                    }
                    catch
                    {
                        // Ignore malformed files
                    }
                }

                return patients;
            }
        }

        public bool Delete(int patientId)
        {
            var path = Path.Combine(_patientRoot, $"{patientId}.json");
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
