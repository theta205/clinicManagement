using System;
using System.Collections.Generic;

namespace Library.Clinic.DTO;

public class PhysicianDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime GraduationDate { get; set; }
    public List<string> Specializations { get; set; } = new List<string>();

    public static implicit operator Models.Physician(PhysicianDTO dto)
    {
        return new Models.Physician
        {
            Id = dto!.Id,
            Name = dto.Name,
            LicenseNumber = dto.LicenseNumber,
            GraduationDate = dto.GraduationDate,
            Specializations = dto.Specializations
        };
    }
}
