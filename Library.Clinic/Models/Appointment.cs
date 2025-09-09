namespace Library.Clinic.Models
{
    public class Appointment
    {
        // Basic Demographics
        public DateTime Date { get; set; }
        public int PatientId {  get; set; }
        public int PhysicianId { get; set; }


        public List<string>? Specializations { get; set; } = new List<string>();


        public override string ToString()
        {
            return $"{LicenseNumber}. {Name}";
        }

    }
}