using System;
using System.Collections.Generic;

namespace Library.Clinic.DTO
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int PhysicianId { get; set; }
        public string? PhysicianName { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public string? Room { get; set; }
        public List<TreatmentDTO> Treatments { get; set; } = new List<TreatmentDTO>();

        public static implicit operator Models.Appointment(AppointmentDTO dto)
        {
            if (dto == null) return null!;
            
            return new Models.Appointment
            {
                Id = dto.Id,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                PatientId = dto.PatientId,
                PatientName = dto.PatientName,
                PhysicianId = dto.PhysicianId,
                PhysicianName = dto.PhysicianName,
                Reason = dto.Reason,
                Notes = dto.Notes,
                Room = dto.Room,
                Treatments = dto.Treatments?.ConvertAll(t => new Models.Treatment
                {
                    Name = t.Name,
                    Cost = t.Cost
                }) ?? new List<Models.Treatment>()
            };
        }
    }
}
