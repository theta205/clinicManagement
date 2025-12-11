using System;
using System.Collections.Generic;

namespace Library.Clinic.Models
{
    public class Appointment
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
        public List<Treatment> Treatments { get; set; } = new List<Treatment>();

        public virtual Patient? Patient { get; set; }
        public virtual Physician? Physician { get; set; }

        public bool IsValidTime()
        {
            if (Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            var appointmentTime = Date.TimeOfDay;
            var startTime = new TimeSpan(8, 0, 0); // 8:00 AM
            var endTime = new TimeSpan(17, 0, 0); // 5:00 PM

            return appointmentTime >= startTime && appointmentTime <= endTime;
        }

        public static bool IsPhysicianAvailable(int physicianId, DateTime startTime, DateTime endTime, int excludeAppointmentId = 0)
        {
            var existingAppointments = Library.Clinic.Services.AppointmentServiceProxy.Current.Appointments
                .Where(a => a.PhysicianId == physicianId && a.Id != excludeAppointmentId);

            foreach (var appointment in existingAppointments)
            {
                if ((startTime < appointment.EndTime && endTime > appointment.StartTime))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return $"Physician: {PhysicianId}. {PhysicianName} and Patient: {PatientId}. {PatientName} at {Date:yyyy-MM-dd HH:mm}";
        }
    }
}
