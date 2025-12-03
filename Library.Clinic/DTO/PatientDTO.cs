using System;

namespace Library.Clinic.DTO
{
    public class PatientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }  // Note: This is the correct property name

        public static implicit operator Models.Patient(PatientDTO dto)
        {
            if (dto == null) return null;
            
            return new Models.Patient
            {
                Id = dto.Id,
                Name = dto.Name,
                SSN = dto.SSN,
                BirthDate = dto.BirthDate  // Map to the correct property name
            };
        }
    }
}
