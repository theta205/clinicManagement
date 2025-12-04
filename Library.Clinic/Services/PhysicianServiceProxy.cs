using Library.Clinic.DTO;
using Library.Clinic.Models;
using Library.Clinic.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Clinic.Services;

public class PhysicianServiceProxy
{
    private static PhysicianServiceProxy? instance;
    private static readonly object _lock = new object();

    public static PhysicianServiceProxy Current
    {
        get
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new PhysicianServiceProxy();
                    }
                }
            }
            return instance;
        }
    }

    private List<PhysicianDTO> physicians;
    private int lastKey;

    private PhysicianServiceProxy()
    {
        // Initialize with sample data
        physicians = new List<PhysicianDTO> {
            new PhysicianDTO { Id = 1, Name = "Dr. Sarah Johnson", LicenseNumber = "MD12345", GraduationDate = new DateTime(2010, 6, 15), Specializations = new List<string> { "Family Medicine", "Internal Medicine" } },
            new PhysicianDTO { Id = 2, Name = "Dr. Michael Chen", LicenseNumber = "MD67890", GraduationDate = new DateTime(2012, 5, 20), Specializations = new List<string> { "Cardiology", "Internal Medicine" } },
            new PhysicianDTO { Id = 3, Name = "Dr. Emily Williams", LicenseNumber = "MD11223", GraduationDate = new DateTime(2015, 8, 10), Specializations = new List<string> { "Pediatrics", "Neonatology" } }
        };
        lastKey = 3;
        
        // Try to load data asynchronously
        _ = LoadPhysiciansAsync();
    }

    private async Task LoadPhysiciansAsync()
    {
        try
        {
            var physiciansData = await new WebRequestHandler().Get("/Physician");
            var loadedPhysicians = JsonConvert.DeserializeObject<List<PhysicianDTO>>(physiciansData) ?? new List<PhysicianDTO>();
            
            physicians = loadedPhysicians;
            lastKey = physicians.Count > 0 ? physicians.Max(p => p.Id) : 0;
        }
        catch (Exception)
        {
            // If web request fails, keep the sample data
        }
    }

    public List<PhysicianDTO> Physicians
    {
        get => physicians;
        private set
        {
            if (physicians != value)
            {
                physicians = value;
            }
        }
    }

    public int LastKey => lastKey;

    public async Task<PhysicianDTO?> AddOrUpdatePhysician(PhysicianDTO physician)
    {
        try
        {
            // Try web service first
            var payload = await new WebRequestHandler().Post("/physician", physician);
            var newPhysician = JsonConvert.DeserializeObject<PhysicianDTO>(payload);
            
            if(newPhysician != null && newPhysician.Id > 0 && physician.Id == 0)
            {
                physicians.Add(newPhysician);
            } else if(newPhysician != null && physician != null && physician.Id > 0 && physician.Id == newPhysician.Id)
            {
                var currentPhysician = physicians.FirstOrDefault(p => p.Id == newPhysician.Id);
                var index = physicians.Count;
                if (currentPhysician != null)
                {
                    index = physicians.IndexOf(currentPhysician);
                    physicians.RemoveAt(index);
                }
                physicians.Insert(index, newPhysician);
            }
            
            return newPhysician;
        }
        catch (Exception)
        {
            // Fallback to local storage if web service fails
            return AddOrUpdatePhysicianLocal(physician);
        }
    }

    private PhysicianDTO? AddOrUpdatePhysicianLocal(PhysicianDTO physician)
    {
        if(physician.Id == 0)
        {
            // New physician - assign ID and add to list
            physician.Id = lastKey + 1;
            physicians.Add(physician);
            lastKey++;
        }
        else
        {
            // Existing physician - update in list
            var existingPhysician = physicians.FirstOrDefault(p => p.Id == physician.Id);
            if (existingPhysician != null)
            {
                var index = physicians.IndexOf(existingPhysician);
                physicians.RemoveAt(index);
                physicians.Insert(index, physician);
            }
            else
            {
                // Physician not found, add it
                physicians.Add(physician);
            }
        }
        
        return physician;
    }

    public async void DeletePhysician(int id) {
        var physicianToRemove = physicians.FirstOrDefault(p => p.Id == id);

        if (physicianToRemove != null)
        {
            physicians.Remove(physicianToRemove);

            try
            {
                // Try web service delete
                await new WebRequestHandler().Delete($"/Physician/{id}");
            }
            catch (Exception)
            {
                // Fallback to local only - physician already removed from list
            }
        }
    }

    public async Task<List<PhysicianDTO>> Search(string query) {
        try
        {
            var physiciansPayload = await new WebRequestHandler()
                .Post("/Physician/Search", new Query(query));

            physicians = JsonConvert.DeserializeObject<List<PhysicianDTO>>(physiciansPayload)
                ?? new List<PhysicianDTO>();
        }
        catch (Exception)
        {
            // Fallback to local search if web service fails
            if (string.IsNullOrWhiteSpace(query))
            {
                // If query is empty, load all physicians
                physicians = physicians.ToList(); // Reset to full list
            }
            else
            {
                // Filter locally
                physicians = physicians
                    .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               p.LicenseNumber.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        return physicians;
    }
}
