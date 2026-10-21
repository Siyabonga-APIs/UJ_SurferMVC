using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using UJ_Surfer.Models;

namespace UJ_Surfer.Data
{
    public interface IEligibilityService
    {
        int CalculateAps(MatricMarksViewModel model);
        int ConvertPercentageToNscLevel(int percentage);
        Task<EligibilityResultsViewModel> GetEligibleProgrammesAsync(MatricMarksViewModel marks);
    }

    public class EligibilityService : IEligibilityService
    {
        private readonly ApplicationDbContext _context;

        public EligibilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int ConvertPercentageToNscLevel(int percentage)
        {
            return percentage switch
            {
                >= 80 => 7,
                >= 70 => 6,
                >= 60 => 5,
                >= 50 => 4,
                >= 40 => 3,
                >= 30 => 2,
                _ => 1
            };
        }

        public int CalculateAps(MatricMarksViewModel model)
        {
            var levels = new List<int>
            {
                ConvertPercentageToNscLevel(model.EnglishMark),
                ConvertPercentageToNscLevel(model.SecondLanguageMark),
                ConvertPercentageToNscLevel(model.MathMark)
            };

            if (model.Elective1Mark.HasValue)
                levels.Add(ConvertPercentageToNscLevel(model.Elective1Mark.Value));

            if (model.Elective2Mark.HasValue)
                levels.Add(ConvertPercentageToNscLevel(model.Elective2Mark.Value));

            if (model.Elective3Mark.HasValue)
                levels.Add(ConvertPercentageToNscLevel(model.Elective3Mark.Value));

            return levels.OrderByDescending(l => l).Take(6).Sum();
        }

        public async Task<EligibilityResultsViewModel> GetEligibleProgrammesAsync(MatricMarksViewModel marks)
        {
            var userAps = CalculateAps(marks);
            var engLevel = ConvertPercentageToNscLevel(marks.EnglishMark);
            var secondLangLevel = ConvertPercentageToNscLevel(marks.SecondLanguageMark);
            var mathLevel = ConvertPercentageToNscLevel(marks.MathMark);

            var userSubjects = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "english", engLevel },
                { marks.SecondLanguageName ?? "second language", secondLangLevel }
            };

            void AddElective(string? name, int? mark)
            {
                if (!string.IsNullOrWhiteSpace(name) && mark.HasValue)
                {
                    var cleanName = name.Trim().ToLower();
                    userSubjects[cleanName] = ConvertPercentageToNscLevel(mark.Value);

                    if (cleanName.Contains("physical science") || cleanName.Contains("physics"))
                    {
                        userSubjects["physical science"] = ConvertPercentageToNscLevel(mark.Value);
                        userSubjects["physical sciences"] = ConvertPercentageToNscLevel(mark.Value);
                    }
                    if (cleanName.Contains("life science") || cleanName.Contains("biology"))
                    {
                        userSubjects["life science"] = ConvertPercentageToNscLevel(mark.Value);
                        userSubjects["life sciences"] = ConvertPercentageToNscLevel(mark.Value);
                    }
                }
            }

            AddElective(marks.Elective1Name, marks.Elective1Mark);
            AddElective(marks.Elective2Name, marks.Elective2Mark);
            AddElective(marks.Elective3Name, marks.Elective3Mark);

            var programmes = await _context.Programmes
                .Include(p => p.Faculty)
                .Include(p => p.SubjectRequirements)
                .ToListAsync();

            var qualifiedList = new List<QualifiedProgrammeDto>();

            foreach (var prog in programmes)
            {
                if (prog.MinimumApsValue.HasValue && userAps < prog.MinimumApsValue.Value)
                {
                    continue;
                }

                bool qualifies = true;

                foreach (var req in prog.SubjectRequirements)
                {
                    var subjName = req.Subject.Trim().ToLower();
                    var reqText = req.Requirement.Trim().ToLower();

                    if (reqText.Contains("not applicable") || string.IsNullOrWhiteSpace(reqText))
                    {
                        continue;
                    }

                    // --- 1. ENGLISH CHECK ---
                    if (subjName.Contains("english"))
                    {
                        int minLevel = ExtractMinimumLevel(reqText);
                        if (minLevel > 0 && engLevel < minLevel)
                        {
                            qualifies = false;
                            break;
                        }
                    }

                    // --- 2. PURE MATHEMATICS CHECK ---
                    else if ((subjName == "mathematics" || subjName.StartsWith("maths"))
                             && !subjName.Contains("literacy")
                             && !subjName.Contains("technical"))
                    {
                        if (marks.SelectedMathType == MathType.Mathematics)
                        {
                            if (reqText.Contains("not accepted"))
                            {
                                qualifies = false;
                                break;
                            }

                            int minLevel = ExtractMinimumLevel(reqText);
                            if (minLevel > 0 && mathLevel < minLevel)
                            {
                                qualifies = false;
                                break;
                            }
                        }
                        else
                        {
                            // User selected Maths Lit or Tech Maths:
                            // Check if programme explicitly allows their math stream
                            bool acceptsUserMath = false;

                            if (marks.SelectedMathType == MathType.MathematicalLiteracy)
                            {
                                acceptsUserMath = prog.SubjectRequirements.Any(r =>
                                    (r.Subject.ToLower().Contains("literacy") || r.Subject.ToLower().Contains("maths lit"))
                                    && !r.Requirement.ToLower().Contains("not accepted"));
                            }
                            else if (marks.SelectedMathType == MathType.TechnicalMathematics)
                            {
                                acceptsUserMath = prog.SubjectRequirements.Any(r =>
                                    (r.Subject.ToLower().Contains("technical mathematics") || r.Subject.ToLower().Contains("tech maths"))
                                    && !r.Requirement.ToLower().Contains("not accepted"));
                            }

                            if (!acceptsUserMath)
                            {
                                qualifies = false; // Pure Maths is strictly required[cite: 86, 88]
                                break;
                            }
                        }
                    }

                    // --- 3. MATHEMATICAL LITERACY CHECK ---
                    else if (subjName.Contains("literacy") || subjName.Contains("maths lit"))
                    {
                        if (marks.SelectedMathType == MathType.MathematicalLiteracy)
                        {
                            if (reqText.Contains("not accepted"))
                            {
                                qualifies = false;
                                break;
                            }

                            int minLevel = ExtractMinimumLevel(reqText);
                            if (minLevel > 0 && mathLevel < minLevel)
                            {
                                qualifies = false;
                                break;
                            }
                        }
                    }

                    // --- 4. TECHNICAL MATHEMATICS CHECK ---
                    else if (subjName.Contains("technical mathematics") || subjName.Contains("tech maths"))
                    {
                        if (marks.SelectedMathType == MathType.TechnicalMathematics)
                        {
                            if (reqText.Contains("not accepted"))
                            {
                                qualifies = false;
                                break;
                            }

                            int minLevel = ExtractMinimumLevel(reqText);
                            if (minLevel > 0 && mathLevel < minLevel)
                            {
                                qualifies = false;
                                break;
                            }
                        }
                    }

                    // --- 5. PHYSICAL SCIENCE / TECHNICAL SCIENCE CHECK ---
                    else if (subjName.Contains("physical science") || subjName.Contains("physics"))
                    {
                        if (!reqText.Contains("not applicable") && !reqText.Contains("not accepted"))
                        {
                            int minLevel = ExtractMinimumLevel(reqText);
                            if (minLevel >= 3)
                            {
                                var matchingKey = userSubjects.Keys.FirstOrDefault(k =>
                                    k.Contains("physical science") || k.Contains("physics") || k.Contains("technical science"));

                                if (matchingKey == null || userSubjects[matchingKey] < minLevel)
                                {
                                    qualifies = false;
                                    break;
                                }
                            }
                        }
                    }

                    // --- 6. LIFE SCIENCES / BIOLOGY CHECK ---
                    else if (subjName.Contains("life science") || subjName.Contains("biology"))
                    {
                        if (!reqText.Contains("not applicable") && !reqText.Contains("not accepted"))
                        {
                            int minLevel = ExtractMinimumLevel(reqText);
                            if (minLevel >= 3)
                            {
                                var matchingKey = userSubjects.Keys.FirstOrDefault(k =>
                                    k.Contains("life science") || k.Contains("biology"));

                                if (matchingKey == null || userSubjects[matchingKey] < minLevel)
                                {
                                    qualifies = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (qualifies)
                {
                    qualifiedList.Add(new QualifiedProgrammeDto
                    {
                        ProgrammeId = prog.Id,
                        FacultyName = prog.Faculty.Name,
                        FacultyAcronym = prog.Faculty.Acronym,
                        Code = prog.Code,
                        Name = prog.Name,
                        QualificationType = prog.QualificationType,
                        Campus = prog.Campus,
                        MinimumApsText = prog.MinimumApsText,
                        MinimumApsValue = prog.MinimumApsValue,
                        Career = prog.Career,
                        SubjectRulesSummary = prog.SubjectRequirements.Select(s => $"{s.Subject}: {s.Requirement}").ToList()
                    });
                }
            }

            return new EligibilityResultsViewModel
            {
                CalculatedAps = userAps,
                EnglishLevel = engLevel,
                SecondLanguageLevel = secondLangLevel,
                MathLevel = mathLevel,
                MathType = marks.SelectedMathType,
                QualifiedProgrammes = qualifiedList.OrderByDescending(p => p.MinimumApsValue).ToList()
            };
        }

        private int ExtractMinimumLevel(string requirement)
        {
            if (string.IsNullOrWhiteSpace(requirement)) return 0;

            // Extracts the lowest acceptable level (e.g. from "4 (50%+) / 5 (60%+)" it gets 4)[cite: 87, 89]
            var match = Regex.Match(requirement, @"[1-7]");
            if (match.Success && int.TryParse(match.Value, out int level))
            {
                return level;
            }
            return 0;
        }
    }
}