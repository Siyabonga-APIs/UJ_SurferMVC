using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UJ_Surfer.Data;
using UJ_Surfer.Models;

namespace UJ_Surfer.Controllers
{
    [Authorize]
    public class MatricController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEligibilityService _eligibilityService;

        public MatricController(ApplicationDbContext context, IEligibilityService eligibilityService)
        {
            _context = context;
            _eligibilityService = eligibilityService;
        }

        // GET: /Matric/MyRecords
        [HttpGet]
        public async Task<IActionResult> MyRecords()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var savedProfile = await _context.StudentMatricProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            // If user has not recorded marks yet, throw them to the entry form
            if (savedProfile == null)
            {
                return RedirectToAction(nameof(Calculate));
            }

            // Map saved profile back to ViewModel
            var marksVm = new MatricMarksViewModel
            {
                EnglishMark = savedProfile.EnglishMark,
                SecondLanguageName = savedProfile.SecondLanguageName,
                SecondLanguageMark = savedProfile.SecondLanguageMark,
                SelectedMathType = savedProfile.SelectedMathType,
                MathMark = savedProfile.MathMark,
                Elective1Name = savedProfile.Elective1Name,
                Elective1Mark = savedProfile.Elective1Mark,
                Elective2Name = savedProfile.Elective2Name,
                Elective2Mark = savedProfile.Elective2Mark,
                Elective3Name = savedProfile.Elective3Name,
                Elective3Mark = savedProfile.Elective3Mark,
                LifeOrientationMark = savedProfile.LifeOrientationMark
            };

            var results = await _eligibilityService.GetEligibleProgrammesAsync(marksVm);
            ViewBag.LastUpdated = savedProfile.LastUpdated;
            return View("Results", results);
        }

        // GET: /Matric/Calculate
        [HttpGet]
        public async Task<IActionResult> Calculate()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var savedProfile = await _context.StudentMatricProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (savedProfile != null)
            {
                // Prepopulate form if they are re-submitting
                var marksVm = new MatricMarksViewModel
                {
                    EnglishMark = savedProfile.EnglishMark,
                    SecondLanguageName = savedProfile.SecondLanguageName,
                    SecondLanguageMark = savedProfile.SecondLanguageMark,
                    SelectedMathType = savedProfile.SelectedMathType,
                    MathMark = savedProfile.MathMark,
                    Elective1Name = savedProfile.Elective1Name,
                    Elective1Mark = savedProfile.Elective1Mark,
                    Elective2Name = savedProfile.Elective2Name,
                    Elective2Mark = savedProfile.Elective2Mark,
                    Elective3Name = savedProfile.Elective3Name,
                    Elective3Mark = savedProfile.Elective3Mark,
                    LifeOrientationMark = savedProfile.LifeOrientationMark
                };
                return View(marksVm);
            }

            return View(new MatricMarksViewModel());
        }

        // POST: /Matric/Calculate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(MatricMarksViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var aps = _eligibilityService.CalculateAps(model);
            var existingProfile = await _context.StudentMatricProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingProfile == null)
            {
                var newProfile = new StudentMatricProfile
                {
                    UserId = userId,
                    EnglishMark = model.EnglishMark,
                    SecondLanguageName = model.SecondLanguageName,
                    SecondLanguageMark = model.SecondLanguageMark,
                    SelectedMathType = model.SelectedMathType,
                    MathMark = model.MathMark,
                    Elective1Name = model.Elective1Name,
                    Elective1Mark = model.Elective1Mark,
                    Elective2Name = model.Elective2Name,
                    Elective2Mark = model.Elective2Mark,
                    Elective3Name = model.Elective3Name,
                    Elective3Mark = model.Elective3Mark,
                    LifeOrientationMark = model.LifeOrientationMark,
                    CalculatedAps = aps,
                    LastUpdated = DateTime.UtcNow
                };
                _context.StudentMatricProfiles.Add(newProfile);
            }
            else
            {
                existingProfile.EnglishMark = model.EnglishMark;
                existingProfile.SecondLanguageName = model.SecondLanguageName;
                existingProfile.SecondLanguageMark = model.SecondLanguageMark;
                existingProfile.SelectedMathType = model.SelectedMathType;
                existingProfile.MathMark = model.MathMark;
                existingProfile.Elective1Name = model.Elective1Name;
                existingProfile.Elective1Mark = model.Elective1Mark;
                existingProfile.Elective2Name = model.Elective2Name;
                existingProfile.Elective2Mark = model.Elective2Mark;
                existingProfile.Elective3Name = model.Elective3Name;
                existingProfile.Elective3Mark = model.Elective3Mark;
                existingProfile.LifeOrientationMark = model.LifeOrientationMark;
                existingProfile.CalculatedAps = aps;
                existingProfile.LastUpdated = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyRecords));
        }
    }
}