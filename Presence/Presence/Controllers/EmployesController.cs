using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
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
        //Upload csv function
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
                var employes = new List<Employe>();

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

                        if (values.Length < 16)
                        {
                            Debug.WriteLine($"Row {row} skipped: Not enough columns.");
                            continue;
                        }

                        try
                        {
                            int.TryParse(values[0], out int departementId);
                            int.TryParse(values[1], out int IdCode_Postal);

                            var employe = new Employe
                            {
                                IdDepartement = departementId,
                                IdCode_Postal = IdCode_Postal,
                                FirstName = values[3],
                                LastName = values[4],
                                BirthDate = TryParseWithDebug(() => ParseDate(values[5]), "BirthDate"),
                                HireDate = TryParseWithDebug(() => ParseDate(values[6]), "HireDate"),
                                Email = values[7],
                                Address = values[8],
                                City = values[9],
                                Region = values[10],
                                Country= values[11],
                                Phone = values[12],
                                Extension = values[13],
                                Photo = values[14],
                                EmployeeNotes = values[15],
                            };

                            Debug.WriteLine($"Row {row} successfully parsed and added to list.");
                            employes.Add(employe);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error parsing row {row}: {ex.Message}");
                            continue;
                        }
                    }
                }

                if (employes.Count > 0)
                {
                    Debug.WriteLine($"Inserting {employes.Count} records into database.");
                    await _context.Employes.AddRangeAsync(employes);
                    await _context.SaveChangesAsync();

                    // **Generate barcode and update database**
                    string rootBarcodePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "barcodes");
                    foreach (var employe in employes)
                    {
                        string barcodeName = BarcodeHelper.GenerateBarcode(employe.Id, rootBarcodePath);
                        employe.Barcode = barcodeName;
                    }

                    await _context.SaveChangesAsync(); // Save the updated barcode names
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
        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            // Load employees with related data
            var employees = await _context.Employes
                .Include(e => e.Departement)
                .Include(e=>e.Postal)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Liste_des_Employes");

                // Add headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Département";
                worksheet.Cells[1, 3].Value = "Code Postal";
                worksheet.Cells[1, 4].Value = "Prénom";
                worksheet.Cells[1, 5].Value = "Nom";
                worksheet.Cells[1, 6].Value = "Date de Naissance";
                worksheet.Cells[1, 7].Value = "Date d'Embauche";
                worksheet.Cells[1, 8].Value = "Email";
                worksheet.Cells[1, 9].Value = "Adresse";
                worksheet.Cells[1, 10].Value = "Ville";
                worksheet.Cells[1, 11].Value = "Région";
                worksheet.Cells[1, 12].Value = "Country";
                worksheet.Cells[1, 13].Value = "Téléphone";
                worksheet.Cells[1, 14].Value = "Extension";
                worksheet.Cells[1, 15].Value = "Photo";
                worksheet.Cells[1, 16].Value = "Notes de l'Employé";
                worksheet.Cells[1, 17].Value = "Barcode";

                // Add data rows
                for (int i = 0; i < employees.Count; i++)
                {
                    var employe = employees[i];
                    worksheet.Cells[i + 2, 1].Value = employe.Id;
                    worksheet.Cells[i + 2, 2].Value = employe.Departement?.NomDepartement;
                    worksheet.Cells[i + 2, 3].Value = employe.Postal?.Code_Postal;
                    worksheet.Cells[i + 2, 4].Value = employe.FirstName;
                    worksheet.Cells[i + 2, 5].Value = employe.LastName;
                    worksheet.Cells[i + 2, 6].Value = employe.BirthDate?.ToString("dd/MM/yyyy") ?? "N/A";
                    worksheet.Cells[i + 2, 7].Value = employe.HireDate?.ToString("dd/MM/yyyy") ?? "N/A";
                    worksheet.Cells[i + 2, 8].Value = employe.Email;
                    worksheet.Cells[i + 2, 9].Value = employe.Address;
                    worksheet.Cells[i + 2, 10].Value = employe.City;
                    worksheet.Cells[i + 2, 11].Value = employe.Region;
                    worksheet.Cells[i + 2, 12].Value = employe.Country;
                    worksheet.Cells[i + 2, 13].Value = employe.Phone;
                    worksheet.Cells[i + 2, 14].Value = employe.Extension;
                    worksheet.Cells[i + 2, 15].Value = employe.Photo;
                    worksheet.Cells[i + 2, 16].Value = employe.EmployeeNotes;
                    worksheet.Cells[i + 2, 17].Value = employe.Barcode ?? "N/A";
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Generate Excel file as a byte array
                var excelFile = package.GetAsByteArray();

                // Return the file
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Liste_des_Employes.xlsx");
            }
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
                
                string barcodeFilename = employe.Barcode;

                if (!string.IsNullOrEmpty(barcodeFilename))
                {
                    
                    string barcodePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "barcodes", barcodeFilename);

                    if (System.IO.File.Exists(barcodePath))
                    {
                        System.IO.File.Delete(barcodePath);
                    }
                }

                // Remove the employee record from the database
                _context.Employes.Remove(employe);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool EmployeExists(int id)
        {
            return _context.Employes.Any(e => e.Id == id);
        }
    }
}
