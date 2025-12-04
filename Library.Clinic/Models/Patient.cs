using System;
using System.Collections.Generic;

namespace Library.Clinic.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string? Address { get; set; }
        public string Race { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public List<string> Diagnoses { get; set; } = new List<string>();
        public List<string> Prescriptions { get; set; } = new List<string>();

        public static implicit operator DTO.PatientDTO(Patient patient)
        {
            return new DTO.PatientDTO
            {
                Id = patient!.Id,
                Name = patient.Name,
                SSN = patient.SSN,
                BirthDate = patient.BirthDate,
                Address = patient.Address,
                Race = patient.Race,
                Gender = patient.Gender,
                Diagnoses = patient.Diagnoses,
                Prescriptions = patient.Prescriptions
            };
        }
    }
}
