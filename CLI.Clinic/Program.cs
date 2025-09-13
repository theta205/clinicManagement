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
                    case "pa":
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

                        DateOnly? birthdate = null;
                        while (true){
                            Console.Write("Enter patient's birth date (mm/dd/yyyy) (optional, press Enter to skip): ");
                            input = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(input)) {
                                // User skipped
                                break;
                            }

                            if (DateOnly.TryParse(input, out DateOnly date)){
                                birthdate = date;
                                break;
                            }
                            else{
                                Console.WriteLine("Invalid date format! Please use (mm/dd/yyyy).\n");
                            }
                        }

                        patient.Birthdate = birthdate;

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
                    case "ph":

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

                        DateOnly? graddate = null;
                        while (true){
                            Console.Write("Enter physician's grad date (mm/dd/yyyy) (optional, press Enter to skip): ");
                            input = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(input)) {
                                // User skipped
                                break;
                            }

                            if (DateOnly.TryParse(input, out DateOnly date)){
                                birthdate = date;
                                break;
                            }
                            else{
                                Console.WriteLine("Invalid date format! Please use (mm/dd/yyyy).\n");
                            }
                        }
                        physician.GradDate = graddate;
                        
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
                        physicians.Add(physician);
                        Console.WriteLine("Physician Added");

                        break;
                    case "A": 
                    case "a":

                        var appointment = new Appointment();
                        input = null;

                        var foundId = false;
                        while (true){
                            Console.WriteLine("Select the id of the physician (format is Id. Name)");
                            Console.WriteLine("Physicians: ");
                            foreach(var ph in physicians){
                                Console.WriteLine(ph);
                            }
                            input = Console.ReadLine();
                            foreach(var ph in physicians){
                                if (input != null && input == ph.LicenseNumber){
                                    foundId = true;
                                    appointment.PhysicianName = ph.Name;
                                }
                            }
                            if(foundId){
                                break;
                            }
                            else{
                                Console.WriteLine("Id not found try again");
                            }
                        }
                        appointment.PhysicianId =  int.Parse(input);;
                        var PhysicianIdForAppointment = appointment.PhysicianId;

                        input = null;
                        foundId = false;
                        while (true){
                            Console.WriteLine("Select the id of the patient (format is Id. Name)");
                            Console.WriteLine("Patients: ");
                            foreach(var p in patients){
                                Console.WriteLine(p);
                            }
                            input = Console.ReadLine();
                            foreach(var p in patients){
                                if (int.Parse(input) == p.Id){
                                    foundId = true;
                                    appointment.PatientName = p.Name;
                                }
                            }
                            if(foundId){
                                break;
                            }
                            else{
                                Console.WriteLine("Id not found try again");
                            }
                        }
                        appointment.PatientId = int.Parse(input);

                        var unavailable = true;

                        while(unavailable){
                            Console.WriteLine("Select the date and time of the appointment");
                            int year, month, day;

                            while (true){
                                Console.Write("Enter the year of appointment: ");
                                string y = Console.ReadLine();
                                if (int.TryParse(y, out year) && year >= 2025){
                                    break;
                                }
                                else{
                                    Console.WriteLine("Invalid year! Please enter a valid year.");
                                }
                            }

                            while (true){
                                Console.Write("Enter the month of appointment as a number (e.g. April -> 4): ");
                                string m = Console.ReadLine();
                                if (int.TryParse(m, out month) && month >= 1 && month <= 12){
                                    break;
                                }
                                else{
                                    Console.WriteLine("Invalid month! Please enter a number between 1 and 12.");
                                }
                            }

                            while (true){
                                Console.Write("Enter the day of appointment: ");
                                string d = Console.ReadLine();
                                if (int.TryParse(d, out day) && day >= 1 && day <= DateTime.DaysInMonth(year, month)){
                                    break;
                                }
                                else{
                                    Console.WriteLine("Invalid day! Please enter a valid day for that month.");
                                }
                            }

                            TimeOnly time = default;
                            var validTime = false;
                            while (!validTime){
                            Console.Write("Enter the time of appointment (e.g. 3:45 PM): ");
                            string t = Console.ReadLine();

                            if (TimeOnly.TryParse(t, out time)){
                                TimeOnly start = new TimeOnly(9, 0);   // 9:00 AM
                                TimeOnly end = new TimeOnly(17, 0);    // 5:00 PM

                                if (time >= start && time <= end){
                                    validTime = true;
                                }
                                else{
                                    Console.WriteLine("Time must be between 9:00 AM and 5:00 PM. Please try again.\n");
                                }
                            }
                            else{
                                Console.WriteLine("Invalid time format! Please try again.\n");
                            }
                        }
                            DateTime appointmentTime = new DateTime(year, month, day, time.Hour, time.Minute, time.Second);
                            
                            var overlap = false;

                            foreach(var a in appointments){
                                if(a.Date == appointmentTime && a.PhysicianId == PhysicianIdForAppointment){
                                    overlap = true;
                                    break;
                                }
                            }
                            if(!overlap) {
                                appointment.Date = appointmentTime;
                                appointments.Add(appointment);
                                Console.WriteLine("Appointment Added");
                                unavailable = false;
                            }
                            else{
                                Console.WriteLine("Appointment Not Added (Doctor is not available then)");
                            }
                        }
                        break;
                    case "L": 
                    case "l":

                        Console.WriteLine("Patients: ");
                        foreach(var p in patients){
                            Console.WriteLine(p);
                        }
                        Console.WriteLine("Physicians: ");
                        foreach(var ph in physicians){
                            Console.WriteLine(ph);
                        }
                        Console.WriteLine("Appointments: ");
                        foreach(var a in appointments){
                            Console.WriteLine(a);
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
