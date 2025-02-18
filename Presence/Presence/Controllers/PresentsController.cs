using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presence.Data;
using Presence.Models;
using static Presence.Controllers.EmployesController;

namespace Presence.Controllers
{
    public class PresentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PresentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Presents
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Presents.Include(p => p.Departement).Include(p => p.Employe).Include(p => p.Shift);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Presents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var present = await _context.Presents
                .Include(p => p.Departement)
                .Include(p => p.Employe)
                .Include(p => p.Shift)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (present == null)
            {
                return NotFound();
            }

            return View(present);
        }

        // GET: Presents/Create
        public IActionResult Create()
        {
            ViewData["IdDepartement"] = new SelectList(_context.Departements, "Id", "NomDepartement");
            ViewData["IdEmploye"] = new SelectList(_context.Employes, "Id", "Nom");
            ViewData["IdShift"] = new SelectList(_context.Shifts, "Id", "ShiftName");
            return View();
        }

        // POST: Presents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdDepartement,IdEmploye,IdShift,Jour,Presente,JourIn,JourOut")] Present present)
        {
            if (ModelState.IsValid)
            {
                _context.Add(present);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDepartement"] = new SelectList(_context.Departements, "Id", "NomDepartement", present.IdDepartement);
            ViewData["IdEmploye"] = new SelectList(_context.Employes, "Id", "Nom", present.IdEmploye);
            ViewData["IdShift"] = new SelectList(_context.Shifts, "Id", "ShiftName", present.IdShift);
            return View(present);
        }

        // GET: Presents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var present = await _context.Presents.FindAsync(id);
            if (present == null)
            {
                return NotFound();
            }
            ViewData["IdDepartement"] = new SelectList(_context.Departements, "Id", "NomDepartement", present.IdDepartement);
            ViewData["IdEmploye"] = new SelectList(_context.Employes, "Id", "Nom", present.IdEmploye);
            ViewData["IdShift"] = new SelectList(_context.Shifts, "Id", "ShiftName", present.IdShift);
            return View(present);
        }

        // POST: Presents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdDepartement,IdEmploye,IdShift,Jour,Presente,JourIn,JourOut")] Present present)
        {
            if (id != present.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(present);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PresentExists(present.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDepartement"] = new SelectList(_context.Departements, "Id", "NomDepartement", present.IdDepartement);
            ViewData["IdEmploye"] = new SelectList(_context.Employes, "Id", "Nom", present.IdEmploye);
            ViewData["IdShift"] = new SelectList(_context.Shifts, "Id", "ShiftName", present.IdShift);
            return View(present);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile csvFile)
        {
            Debug.WriteLine("Upload method started.");

            if (csvFile == null || csvFile.Length == 0)
            {
                Debug.WriteLine("Validation failed: File is null or empty.");
                ModelState.AddModelError("File", "Please upload a valid CSV file.");
                return RedirectToAction("Index");
            }

            if (Path.GetExtension(csvFile.FileName).ToLower() != ".csv")
            {
                Debug.WriteLine("Validation failed: File is not a CSV.");
                ModelState.AddModelError("File", "Only CSV files are allowed.");
                return RedirectToAction("Index");
            }

            try
            {
                var presences = new List<Present>();

                using (var streamReader = new StreamReader(csvFile.OpenReadStream()))
                {
                    string line;
                    int row = 0;

                    while ((line = await streamReader.ReadLineAsync()) != null)
                    {
                        row++;
                        Debug.WriteLine($"Processing row {row}: {line}");

                        if (row == 1)
                        {
                            Debug.WriteLine("Skipping header row.");
                            continue;
                        }

                        var values = line.Split(',');

                        if (values.Length < 7)
                        {
                            Debug.WriteLine($"Row {row} skipped: Not enough columns.");
                            continue;
                        }

                        try
                        {
                            Debug.WriteLine($"Searching for Departement: {values[0]}");
                            if (int.TryParse(values[0], out int departementId))
                            {
                                var departement = await _context.Departements
                                    .FirstOrDefaultAsync(a => a.Id == departementId);
                            }
                            else
                            {
                                Debug.WriteLine("Invalid departement ID");
                            }
;

                            Debug.WriteLine($"Searching for Employe: {values[1]}");
                            if (int.TryParse(values[1], out int employeId))
                            {
                                var employe = await _context.Employes
                                    .FirstOrDefaultAsync(a => a.Id == employeId);
                            }
                            else
                            {
                                Debug.WriteLine("Invalid employe ID");
                            }
                            if (int.TryParse(values[2], out int shiftId))
                            {
                                var shifts = await _context.Shifts
                                    .FirstOrDefaultAsync(a => a.Id == shiftId);
                            }
                            else
                            {
                                Debug.WriteLine("Invalid shift ID");
                            }

                         

                        

                      

                           

                            var present = new Present
                            {
                                IdDepartement = departementId,
                                IdEmploye = employeId,
                                IdShift = shiftId,
                                Jour = TryParseWithDebug(() => ParseDate(values[3]), "Jour"),
                                JourIn = values[4],
                                JourOut = values[5],
                                Presente = false
                            };

                            Debug.WriteLine($"Row {row} successfully parsed and added to list.");
                            presences.Add(present);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error parsing row {row}: {ex.Message}");
                            continue;
                        }
                    }
                }

                if (presences.Count > 0)
                {
                    Debug.WriteLine($"Inserting {presences.Count} records into database.");
                    await _context.Presents.AddRangeAsync(presences);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Debug.WriteLine("No valid rows found. Nothing to insert.");
                }

                Debug.WriteLine("Upload method completed successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error uploading file: {ex.Message}");
                ModelState.AddModelError("File", "An error occurred during the upload process.");
                return RedirectToAction("Index");
            }
        }


        private DateOnly? ParseDate(string dateString)
        {
            // Define multiple possible formats to parse
            var formats = new[] { "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy", "dd/MM/yyyy" };

            // Trim input to remove extra spaces
            dateString = dateString.Trim();

            Console.WriteLine($"Attempting to parse date: '{dateString}'");

            if (DateTime.TryParseExact(dateString, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                Console.WriteLine($"Successfully parsed date: {parsedDate}");
                return DateOnly.FromDateTime(parsedDate);
            }

            // Log failure
            Console.WriteLine($"Failed to parse date: '{dateString}'");
            return null;
        }

        private bool ParseBoolean(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            value = value.Trim().ToLower();
            return value switch
            {
                "1" or "true" or "yes" => true,
                "0" or "false" or "no" => false,
                _ => false,
            };
        }
        private T TryParseWithDebug<T>(Func<T> func, string fieldName)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors du traitement du champ '{fieldName}': {ex.Message}");
                throw;
            }
        }
        // GET: Presents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var present = await _context.Presents
                .Include(p => p.Departement)
                .Include(p => p.Employe)
                .Include(p => p.Shift)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (present == null)
            {
                return NotFound();
            }

            return View(present);
        }

        // POST: Presents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var present = await _context.Presents.FindAsync(id);
            if (present != null)
            {
                _context.Presents.Remove(present);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PresentExists(int id)
        {
            return _context.Presents.Any(e => e.Id == id);
        }
    }
}
