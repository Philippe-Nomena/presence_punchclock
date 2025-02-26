using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presence.Data;
using Presence.Models;

namespace Presence.Controllers
{
    public class EmployesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employes.Include(e => e.Departement).Include(e => e.Postal);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employe = await _context.Employes
                .Include(e => e.Departement)
                .Include(e => e.Postal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employe == null)
            {
                return NotFound();
            }

            return View(employe);
        }

        public class BarcodeHelper
        {
            public static string GenerateBarcode(int employeId, string rootBarcode)
            {
                if (employeId <= 0)
                    throw new ArgumentException("Employe ID must be a positive integer.", nameof(employeId));

                if (!Directory.Exists(rootBarcode))
                {
                    Directory.CreateDirectory(rootBarcode);
                }

                string fileName = $"{employeId}_barcode.png";
                string filePath = Path.Combine(rootBarcode, fileName);

                try
                {
                    var writer = new ZXing.SkiaSharp.BarcodeWriter
                    {
                        Format = ZXing.BarcodeFormat.CODE_39,
                        Options = new ZXing.Common.EncodingOptions
                        {
                            Height = 100,
                            Width = 300,
                            Margin = 2,
                            PureBarcode=true
                        }
                    };

                    // Créer le fichier
                    using (var image = writer.Write(employeId.ToString()))
                    using (var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100))
                    {
                        // Crée un FileStream pour écrire les données dans le fichier
                        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            data.SaveTo(stream);
                        }
                    }

                    return fileName;

                }
                catch (UnauthorizedAccessException ex)
                {
                    // Gérer les problèmes de permission
                    throw new InvalidOperationException($"Access denied: {ex.Message}", ex);
                }
                catch (IOException ex)
                {
                    // Gérer les erreurs d'IO (fichier déjà ouvert, etc.)
                    throw new InvalidOperationException($"File I/O error: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    // Gérer les erreurs générales
                    throw new InvalidOperationException($"Error generating barcode: {ex.Message}", ex);
                }
            }
        }

        [HttpGet("GetBarcode/{employeId}")]
        public IActionResult GetBarcode(int employeId)
        {
            string rootBarcodePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "barcodes");

            // Valider et créer le dossier si nécessaire
            if (!Directory.Exists(rootBarcodePath))
            {
                Directory.CreateDirectory(rootBarcodePath);
            }

            string fileName = $"{employeId}_barcode.png";
            string filePath = Path.Combine(rootBarcodePath, fileName);

            // Vérifier si le fichier existe
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Barcode not found.");
            }

            // Lire et retourner le fichier
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "image/png", fileName);
        }

        // GET: Employes/Create
        public IActionResult Create()
        {
            ViewData["IdDepartement"] = new SelectList(_context.Departements, "Id", "NomDepartement");
            ViewData["IdCode_Postal"] = new SelectList(_context.Postals, "Id", "Code_Postal");
            return View();
        }

        // POST: Employes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdCode_Postal,IdDepartement,FirstName,LastName,Sexe,BirthDate,HireDate,Email,Address,City,Region,Country,Phone,Extension,Photo,EmployeeNotes,Barcode")] Employe employe)
        {
            try
            {
                _context.Add(employe);
                await _context.SaveChangesAsync(); 

                string rootBarcodePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "barcodes");

                string barcodeName = BarcodeHelper.GenerateBarcode(employe.Id, rootBarcodePath);
                employe.Barcode = barcodeName;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to generate barcode. Error: {ex.Message}");
                Console.WriteLine($"Barcode generation failed: {ex.Message}"); // Log error
                return View(employe);
            }

        }

        // GET: Employes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
            {
                return NotFound();
            }
            ViewData["IdDepartement"] = new SelectList(_context.Departements, "Id", "NomDepartement", employe.IdDepartement);
            ViewData["IdCode_Postal"] = new SelectList(_context.Postals, "Id", "Code_Postal", employe.IdCode_Postal);
            return View(employe);
        }

        // POST: Employes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdCode_Postal,IdDepartement,FirstName,LastName,Sexe,BirthDate,HireDate,Email,Address,City,Region,Country,Phone,Extension,Photo,EmployeeNotes,Barcode")] Employe employe)
        {
            if (id != employe.Id)
            {
                return NotFound();
            }

           
                try
                {
                    _context.Update(employe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeExists(employe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
       
            ViewData["IdDepartement"] = new SelectList(_context.Departements, "Id", "NomDepartement", employe.IdDepartement);
            ViewData["IdCode_Postal"] = new SelectList(_context.Postals, "Id", "Code_Postal", employe.IdCode_Postal);
            return View(employe);
        }

        // GET: Employes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employe = await _context.Employes
                .Include(e => e.Departement)
                .Include(e => e.Postal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employe == null)
            {
                return NotFound();
            }

            return View(employe);
        }

        // POST: Employes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employe = await _context.Employes.FindAsync(id);
            if (employe != null)
            {
                _context.Employes.Remove(employe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeExists(int id)
        {
            return _context.Employes.Any(e => e.Id == id);
        }
    }
}
