namespace Library.Clinic.Models
{
    public class Physician
    {
        // Basic Demographics
        public string Name { get; set; }
        public DateOnly? GradDate {  get; set; }
        public int LicenseNumber { get; set; }


        public List<string>? Specializations { get; set; } = new List<string>();


        public override string ToString()
        {
            return $"{LicenseNumber}. {Name}";
        }

    }
}