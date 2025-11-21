namespace Library.Clinic.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int PatientId {  get; set; }
        public string? PatientName {  get; set; }
        public int PhysicianId { get; set; }
        public string? PhysicianName { get; set; }

        public override string ToString()
        {
            return $"Physician: {PhysicianId}. {PhysicianName} and Patient: {PatientId}. {PatientName} at {Date}";
        }

    }
}