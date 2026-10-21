using System.ComponentModel.DataAnnotations;

namespace UJ_Surfer.Models
{
    public class MatricMarksViewModel
    {
        // --- 1. Language of Teaching & Learning (English) ---
        [Required(ErrorMessage = "English mark is required")]
        [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100%")]
        [Display(Name = "English (Home Language or First Additional)")]
        public int EnglishMark { get; set; }

        // --- 2. Second Official Language ---
        [Display(Name = "Second Official Language")]
        public string? SecondLanguageName { get; set; } = "isiZulu / Afrikaans / Sepedi / other";

        [Required(ErrorMessage = "Second language mark is required")]
        [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100%")]
        [Display(Name = "Second Language Mark")]
        public int SecondLanguageMark { get; set; }

        // --- 3. Mathematics ---
        [Required(ErrorMessage = "Mathematics stream is required")]
        [Display(Name = "Mathematics Subject Choice")]
        public MathType SelectedMathType { get; set; } = MathType.Mathematics;

        [Required(ErrorMessage = "Math mark is required")]
        [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100%")]
        [Display(Name = "Mathematics Mark")]
        public int MathMark { get; set; }

        // --- 4. Elective Subject 1 ---
        [Display(Name = "Elective Subject 1")]
        public string? Elective1Name { get; set; }

        [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100%")]
        [Display(Name = "Elective 1 Mark")]
        public int? Elective1Mark { get; set; }

        // --- 5. Elective Subject 2 ---
        [Display(Name = "Elective Subject 2")]
        public string? Elective2Name { get; set; }

        [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100%")]
        [Display(Name = "Elective 2 Mark")]
        public int? Elective2Mark { get; set; }

        // --- 6. Elective Subject 3 ---
        [Display(Name = "Elective Subject 3")]
        public string? Elective3Name { get; set; }

        [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100%")]
        [Display(Name = "Elective 3 Mark")]
        public int? Elective3Mark { get; set; }

        // --- 7. Life Orientation ---
        [Range(0, 100, ErrorMessage = "Mark must be between 0 and 100%")]
        [Display(Name = "Life Orientation (Excluded from APS calculation)")]
        public int? LifeOrientationMark { get; set; }
    }

    public enum MathType
    {
        Mathematics = 1,
        [Display(Name = "Mathematical Literacy")]
        MathematicalLiteracy = 2,
        [Display(Name = "Technical Mathematics")]
        TechnicalMathematics = 3
    }

    public class EligibilityResultsViewModel
    {
        public int CalculatedAps { get; set; }
        public int EnglishLevel { get; set; }
        public int SecondLanguageLevel { get; set; }
        public int MathLevel { get; set; }
        public MathType MathType { get; set; }
        public int TotalEligibleCount => QualifiedProgrammes.Count;
        public List<QualifiedProgrammeDto> QualifiedProgrammes { get; set; } = new();
    }

    public class QualifiedProgrammeDto
    {
        public int ProgrammeId { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public string FacultyAcronym { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string QualificationType { get; set; } = string.Empty;
        public string Campus { get; set; } = string.Empty;
        public string MinimumApsText { get; set; } = string.Empty;
        public int? MinimumApsValue { get; set; }
        public string Career { get; set; } = string.Empty;
        public List<string> SubjectRulesSummary { get; set; } = new();
    }
}