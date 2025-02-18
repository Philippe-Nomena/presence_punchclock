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


        //Upload 


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile csvFile)
        {
           
            if (csvFile == null || csvFile.Length == 0)
            {
                ModelState.AddModelError("File", "Please upload a valid CSV file.");
                return RedirectToAction("Index");
            }

            if (Path.GetExtension(csvFile.FileName).ToLower() != ".csv")
            {
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
                        if (row == 1) continue; // Skip the header row

                        var values = line.Split(',');

                        try
                        {


                            var departement = _context.Departements.FirstOrDefault(a => a.NomDepartement.Equals(values[1]));
                            var employe = _context.Employes.FirstOrDefault(a => a.Nom.Equals(values[13]));
                            var shift = _context.Shifts.FirstOrDefault(a => a.ShiftName.Equals(values[14]));

                            var present = new Present
                            {
                                IdDepartement = departement?.Id ?? 0,
                                IdEmploye = employe?.Id ?? 0,
                                IdShift = shift?.Id ?? 0,
                                Jour = TryParseWithDebug(() => ParseDate(values[4]), "Jour"),
                                Presente = TryParseWithDebug(() => ParseBoolean(values[5]), "Presente"),
                                JourIn = TryParseWithDebug(() => values[6], "JourIn"),
                                JourOut = TryParseWithDebug(() => values[7], "JourOut"),

                                // Assuming values[6] and values[7] are the strings that you're parsing


                            };

                            Debug.WriteLine("==========================================================");
                            Debug.WriteLine("===================ARY ATO KA Mandalo pory ato ve =======================================", present);

                            Debug.WriteLine("==========================================================");
                            presences.Add(present);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error parsing row {row}: {ex.Message}");
                            ModelState.AddModelError("File", $"Error parsing row {row}: {ex.Message}");
                            continue;
                        }
                    }
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    foreach (var present in presences)
                    {
                        Debug.WriteLine("==========================================================");
                        Debug.WriteLine("===================FARANY ATO Mandalo pory ato ve =======================================", present);

                        Debug.WriteLine("==========================================================");
                        _context.Presents.Add(present);
                        await _context.SaveChangesAsync();

                        // Optionally, you can generate barcodes or any other post-processing tasks here
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Debug.WriteLine($"Error during bulk insert: {ex.Message}");
                    ModelState.AddModelError("File", "An error occurred during the upload process.");
                    return RedirectToAction("Index");
                }

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
