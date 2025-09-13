using System;
using Library.Clinic.Models;


namespace CLI.Clinic
{
    internal class Program
    {
        static void Main(string[] args)
        {
             Console.WriteLine("Welcome to CLinic!");
            List<Patient?> patients = new List<Patient?>();
            List<Physician?> physicians = new List<Physician?>();
            List<Appointment?> appointments = new List<Appointment?>();
            bool cont = true;
            do
            {
                Console.WriteLine("Pa. Create a Patient");
                Console.WriteLine("Ph. Create a Physician");
                Console.WriteLine("A. Create a Appointment");
                Console.WriteLine("L. List all Patients, Physicians, and Appointments");
                Console.WriteLine("Q. Quit");

                var userChoice = Console.ReadLine();
                switch (userChoice)
                {
                    case "Pa":
                        var patient = new Patient();
                        string? input = null;
                        while (string.IsNullOrWhiteSpace(input)){
                            Console.WriteLine("Enter the patient's name (required):");
                            input = Console.ReadLine();
                        }
                        patient.Name = input;

                        Console.WriteLine("Enter the patients address  (optional):");
                        input = Console.ReadLine();
                        patient.Address = string.IsNullOrWhiteSpace(input) ? null : input;

                        Console.WriteLine("Enter the patients gender  (optional):");
                        input = Console.ReadLine();
                        patient.Gender = string.IsNullOrWhiteSpace(input) ? null : input;

                        Console.WriteLine("Enter the patients race  (optional):");
                        input = Console.ReadLine();
                        patient.Race = string.IsNullOrWhiteSpace(input) ? null : input;

                        Console.Write("Enter patients birth date (yyyy-MM-dd) (optional): ");
                        input = Console.ReadLine();
                        patient.Birthdate = DateOnly.TryParse(input, out var date) ? date : null;

                        patient.Prescriptions ??= new List<string>();
                        Console.WriteLine("Enter prescriptions one by one (press Enter on an empty line to finish):");
                        while (true)
                        {
                            Console.Write("Prescription: ");
                            input = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(input))
                                break; // stop if user presses Enter without typing anything

                            patient.Prescriptions.Add(input);
                        }

                        patient.Diagnoses ??= new List<string>();
                        Console.WriteLine("Enter diagnoses one by one (press Enter on an empty line to finish):");
                        while (true)
                        {
                            Console.Write("Diagnoses: ");
                            input = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(input))
                                break; // stop if user presses Enter without typing anything

                            patient.Diagnoses.Add(input);
                        }
                        
                        var maxId = -1;
                        if (patients.Any())
                        {
                            maxId = patients.Select(b => b?.Id ?? -1).Max();
                        } else
                        {
                            maxId = 0;
                        }
                        patient.Id = ++maxId;
                        patients.Add(patient);

                        Console.WriteLine("Patient Added");

                        break;
                    case "Ph": 

                        var physician = new Physician();
                        input = null;
                        while (string.IsNullOrWhiteSpace(input)){
                            Console.WriteLine("Enter the physician's name (required):");
                            input = Console.ReadLine();
                        }
                        physician.Name = input;

                        input = null;
                        while (string.IsNullOrWhiteSpace(input)){
                            Console.WriteLine("Enter the physician's license number (required):");
                            input = Console.ReadLine();
                        }
                        physician.LicenseNumber = input;

                        Console.Write("Enter physician's grad date (yyyy-MM-dd) (optional): ");
                        input = Console.ReadLine();
                        physician.GradDate = DateOnly.TryParse(input, out var graddate) ? graddate : null;
                        
                        physician.Specializations ??= new List<string>();
                        Console.WriteLine("Enter specializations one by one (press Enter on an empty line to finish):");
                        while (true)
                        {
                            Console.Write("Specialization: ");
                            input = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(input))
                                break; // stop if user presses Enter without typing anything

                            physician.Specializations.Add(input);
                        }
                        
                        Console.WriteLine("Physician Added");

                        break;
                    case "A": 
                    case "a":
                        break;
                    case "L": 
                    case "l":

                        Console.WriteLine("Patients: ");
                        foreach(var patient in patients){
                            Console.WriteLine(patient);
                        }
                        Console.WriteLine("Physicians: ");
                        foreach(var physicians in physicians){
                            Console.WriteLine(physicians);
                        }
                        Console.WriteLine("Appointments: ");
                        foreach(var appointment in appointments){
                            Console.WriteLine(appointment);
                        }
                        break;
                    case "Q": 
                    case "q":
                        cont = false;
                        break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }

            } while (cont);
        }
    }
}
