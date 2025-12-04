using Library.Clinic.DTO;
using Library.Clinic.Models;
using Newtonsoft.Json;
using Library.Clinic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Clinic.Services
{
    public class AppointmentServiceProxy
    {
        private static object _lock = new object();
        private static AppointmentServiceProxy? instance;

        public static AppointmentServiceProxy Current
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new AppointmentServiceProxy();
                    }
                }
                return instance;
            }
        }

        // THIS list is your REAL storage.
        // Never overwrite it during search.
        private List<AppointmentDTO> allAppointments = new List<AppointmentDTO>();

        private AppointmentServiceProxy()
        {
            // Initialize with sample demo data
            allAppointments = new List<AppointmentDTO> {
                new AppointmentDTO { 
                    Id = 1, 
                    Date = new DateTime(2024, 10, 9),
                    StartTime = new DateTime(2024, 10, 9, 9, 0, 0),
                    EndTime = new DateTime(2024, 10, 9, 10, 0, 0),
                    PatientId = 1,
                    PatientName = "John Doe",
                    PhysicianId = 1,
                    PhysicianName = "Dr. Smith",
                    Reason = "Annual checkup",
                    Notes = "Patient reports feeling well"
                },
                new AppointmentDTO { 
                    Id = 2, 
                    Date = new DateTime(2024, 10, 10),
                    StartTime = new DateTime(2024, 10, 10, 14, 0, 0),
                    EndTime = new DateTime(2024, 10, 10, 15, 0, 0),
                    PatientId = 2,
                    PatientName = "Jane Smith",
                    PhysicianId = 1,
                    PhysicianName = "Dr. Smith",
                    Reason = "Follow-up",
                    Notes = "Review test results"
                }
            };
            Appointments = allAppointments;
        }

        // Public-facing list (filtered or sorted)
        public List<AppointmentDTO> Appointments { get; private set; } = new List<AppointmentDTO>();

        // Always use ALL appointments for ID assignment
        private int lastKey
        {
            get
            {
                if (allAppointments.Any())
                {
                    return allAppointments.Select(x => x.Id).Max();
                }
                return 0;
            }
        }

        public async Task<AppointmentDTO?> AddOrUpdateAppointment(AppointmentDTO appointment)
        {
            try
            {
                // Try web service first
                var payload = await new WebRequestHandler().Post("/appointment", appointment);
                var newAppointment = JsonConvert.DeserializeObject<AppointmentDTO>(payload);
                
                if (newAppointment != null && newAppointment.Id > 0 && appointment.Id == 0)
                {
                    // New appointment returned with a real ID from server
                    allAppointments.Add(newAppointment);
                }
                else if (newAppointment != null && appointment != null && appointment.Id > 0 && appointment.Id == newAppointment.Id)
                {
                    // Edit: replace existing appointment in list
                    var currentAppointment = allAppointments.FirstOrDefault(a => a.Id == newAppointment.Id);
                    var index = allAppointments.Count;
                    if (currentAppointment != null)
                    {
                        index = allAppointments.IndexOf(currentAppointment);
                        allAppointments.RemoveAt(index);
                    }
                    allAppointments.Insert(index, newAppointment);
                }
                else
                {
                    // Server returned Id = 0 or invalid response, use local fallback
                    return AddOrUpdateAppointmentLocal(appointment);
                }
                
                return newAppointment;
            }
            catch (Exception)
            {
                // Fallback to local storage if web service fails
                return AddOrUpdateAppointmentLocal(appointment);
            }
        }

        private AppointmentDTO? AddOrUpdateAppointmentLocal(AppointmentDTO appointment)
        {
            if (appointment == null) return null;

            var isAdd = false;
            if (appointment.Id <= 0)
            {
                appointment.Id = lastKey + 1;
                isAdd = true;
            }

            if (isAdd)
            {
                allAppointments.Add(appointment);
            }
            else
            {
                // Update existing appointment
                var existingAppointment = allAppointments.FirstOrDefault(a => a.Id == appointment.Id);
                if (existingAppointment != null)
                {
                    var index = allAppointments.IndexOf(existingAppointment);
                    allAppointments.RemoveAt(index);
                    allAppointments.Insert(index, appointment);
                }
            }

            Appointments = new List<AppointmentDTO>(allAppointments);
            return appointment;
        }

        public async Task DeleteAppointment(int appointmentId)
        {
            try
            {
                // Try web service first
                await new WebRequestHandler().Delete($"/appointment/{appointmentId}");
                
                // Remove from local list regardless of web service success
                var appointment = allAppointments.FirstOrDefault(a => a.Id == appointmentId);
                if (appointment != null)
                {
                    allAppointments.Remove(appointment);
                    Appointments = new List<AppointmentDTO>(allAppointments);
                }
            }
            catch (Exception)
            {
                // Fallback to local storage if web service fails
                var appointment = allAppointments.FirstOrDefault(a => a.Id == appointmentId);
                if (appointment != null)
                {
                    allAppointments.Remove(appointment);
                    Appointments = new List<AppointmentDTO>(allAppointments);
                }
            }
        }

        public void Refresh()
        {
            Appointments = new List<AppointmentDTO>(allAppointments);
        }
    }
}