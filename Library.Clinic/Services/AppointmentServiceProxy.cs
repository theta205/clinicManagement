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

        private List<AppointmentDTO> allAppointments = new List<AppointmentDTO>();

        private AppointmentServiceProxy()
        {
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

        public List<AppointmentDTO> Appointments { get; private set; } = new List<AppointmentDTO>();

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
                var payload = await new WebRequestHandler().Post("/appointment", appointment);
                var newAppointment = JsonConvert.DeserializeObject<AppointmentDTO>(payload);
                
                if (newAppointment != null && newAppointment.Id > 0 && appointment.Id == 0)
                {
                    allAppointments.Add(newAppointment);
                }
                else if (newAppointment != null && appointment != null && appointment.Id > 0 && appointment.Id == newAppointment.Id)
                {
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
                    return AddOrUpdateAppointmentLocal(appointment);
                }
                
                return newAppointment;
            }
            catch (Exception)
            {
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
                await new WebRequestHandler().Delete($"/appointment/{appointmentId}");
                
                var appointment = allAppointments.FirstOrDefault(a => a.Id == appointmentId);
                if (appointment != null)
                {
                    allAppointments.Remove(appointment);
                    Appointments = new List<AppointmentDTO>(allAppointments);
                }
            }
            catch (Exception)
            {
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