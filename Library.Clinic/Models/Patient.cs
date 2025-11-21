using System;
using System.Collections.Generic;

namespace Library.Clinic.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public List<string> Diagnoses { get; set; } = new();
        public List<string> Prescriptions { get; set; } = new();
        public override string ToString()
        {
            return $"{Id}. {Name}";
        }

    }
}