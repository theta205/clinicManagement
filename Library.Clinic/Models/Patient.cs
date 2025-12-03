using System;

namespace Library.Clinic.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string? Address { get; set; }

        public static implicit operator DTO.PatientDTO(Patient patient)
        {
            if (patient == null) return null!;
            
            return new DTO.PatientDTO
            {
                Id = patient.Id,
                Name = patient.Name,
                SSN = patient.SSN,
                BirthDate = patient.BirthDate,
                Address = patient.Address
            };
        }
    }
}
