using System.Collections.Generic;

namespace Library.Clinic.Models
{
    public class Physician
    {
        public int Id { get; set; }
        // Basic Demographics
        public string Name { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime GraduationDate { get; set; }
        public List<string> Specializations { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"{LicenseNumber}. {Name}";
        }

        public static implicit operator DTO.PhysicianDTO(Physician physician)
        {
            return new DTO.PhysicianDTO
            {
                Id = physician!.Id,
                Name = physician.Name,
                LicenseNumber = physician.LicenseNumber,
                GraduationDate = physician.GraduationDate,
                Specializations = physician.Specializations
            };
        }
    }
}