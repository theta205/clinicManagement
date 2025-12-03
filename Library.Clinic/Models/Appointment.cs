using System;

namespace Library.Clinic.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int PhysicianId { get; set; }
        public string? PhysicianName { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }

        // Navigation property
        public virtual Patient? Patient { get; set; }

        public override string ToString()
        {
            return $"Physician: {PhysicianId}. {PhysicianName} and Patient: {PatientId}. {PatientName} at {Date}";
        }
    }
}
