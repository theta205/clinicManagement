using System;

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

        // Navigation property
        public virtual Patient? Patient { get; set; }
        public virtual Physician? Physician { get; set; }

        public bool IsValidTime()
        {
            // Check if appointment is on Monday-Friday
            if (Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            // Check if appointment is between 8am and 5pm
            var appointmentTime = Date.TimeOfDay;
            var startTime = new TimeSpan(8, 0, 0); // 8:00 AM
            var endTime = new TimeSpan(17, 0, 0); // 5:00 PM

            return appointmentTime >= startTime && appointmentTime <= endTime;
        }

        public static bool IsPhysicianAvailable(int physicianId, DateTime startTime, DateTime endTime, int excludeAppointmentId = 0)
        {
            // This would typically check against a database
            // For now, we'll implement a basic check using the AppointmentServiceProxy
            var existingAppointments = Library.Clinic.Services.AppointmentServiceProxy.Current.Appointments
                .Where(a => a.PhysicianId == physicianId && a.Id != excludeAppointmentId);

            foreach (var appointment in existingAppointments)
            {
                // Check for overlapping appointments
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
