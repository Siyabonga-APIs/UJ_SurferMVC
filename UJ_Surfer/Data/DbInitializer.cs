using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UJ_Surfer.Models;

namespace UJ_Surfer.Data
{
    public static class DbInitializer
    {
        public static async Task SeedProspectusDataAsync(ApplicationDbContext context, IWebHostEnvironment env)
        {
            // Don't re-seed if data already exists
            if (await context.Faculties.AnyAsync())
            {
                return;
            }

            var jsonFolderPath = Path.Combine(env.ContentRootPath, "Data", "JSON");
            if (!Directory.Exists(jsonFolderPath))
            {
                return;
            }

            var jsonFiles = Directory.GetFiles(jsonFolderPath, "*.json");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
            };

            foreach (var filePath in jsonFiles)
            {
                var jsonString = await File.ReadAllTextAsync(filePath);
                var facultyDto = JsonSerializer.Deserialize<FacultySeedDto>(jsonString, options);

                if (facultyDto == null) continue;

                var faculty = new Faculty
                {
                    Name = facultyDto.Name,
                    Acronym = facultyDto.Acronym,
                    GeneralNotes = facultyDto.GeneralNotes,
                    Contacts = facultyDto.Contacts.Select(c => new Contact
                    {
                        Campus = c.Campus,
                        Telephone = c.Telephone,
                        Email = c.Email
                    }).ToList(),
                    Programmes = facultyDto.Programmes.Select(p => new Programme
                    {
                        QualificationType = p.QualificationType,
                        Category = p.Category,
                        Code = p.Code,
                        Name = p.Name,
                        Campus = p.Campus,
                        MinimumApsText = p.MinimumApsText,
                        MinimumApsValue = p.MinimumApsValue,
                        Career = p.Career,
                        Notes = p.Notes,
                        SubjectRequirements = p.SubjectRequirements.Select(s => new SubjectRequirement
                        {
                            Subject = s.Subject,
                            Requirement = s.Requirement
                        }).ToList()
                    }).ToList()
                };

                context.Faculties.Add(faculty);
            }

            await context.SaveChangesAsync();
        }
    }

    public class FacultySeedDto
    {
        public string Name { get; set; } = string.Empty;
        public string Acronym { get; set; } = string.Empty;
        public string? GeneralNotes { get; set; }
        public List<ContactSeedDto> Contacts { get; set; } = new();
        public List<ProgrammeSeedDto> Programmes { get; set; } = new();
    }

    public class ContactSeedDto
    {
        public string? Campus { get; set; }
        public string Telephone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ProgrammeSeedDto
    {
        public string QualificationType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Campus { get; set; } = string.Empty;
        public string MinimumApsText { get; set; } = string.Empty;
        public int? MinimumApsValue { get; set; }
        public string Career { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<SubjectRequirementSeedDto> SubjectRequirements { get; set; } = new();
    }

    public class SubjectRequirementSeedDto
    {
        public string Subject { get; set; } = string.Empty;
        public string Requirement { get; set; } = string.Empty;
    }

}