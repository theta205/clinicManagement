namespace Library.Clinic.Models
{
    public class Patient
    {
        // Basic Demographics
        public string? Name { get; set; }
        public string? Address {  get; set; }
        public string? Birthdate {  get; set; }
        public string? Race {  get; set; }
        public string? Gender {  get; set; }
        public int Id { get; set; }


        // Medical Stats

        public List<string>? Prescriptions { get; set; } = new List<string>();
        public List<string>? Diagnoses { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"{Id}. {Name}";
        }

    }
}