using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using SGCApp.Data;
using SGCApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Newtonsoft.Json;
using SGCApp.Attributes;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using X.PagedList;
using X.PagedList.Mvc.Core;



namespace SGCApp.Controllers
{
    [Authorize]
    public class ProcedureController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _procedureDirectory = "wwwroot/procedures";
        private const int PageSize = 8;

        public ProcedureController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString, int? page, string searchOption)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var proceduresQuery = _context.Procedures.Include(p => p.ModifiedByUser).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchOption)
                {
                    case "id":
                        proceduresQuery = proceduresQuery.Where(p => p.Id.ToString().Contains(searchString));
                        break;
                    case "title":
                        proceduresQuery = proceduresQuery.Where(p => p.Title.Contains(searchString));
                        break;
                    case "date":
                        proceduresQuery = proceduresQuery.Where(p => p.CreatedAt.ToString().Contains(searchString));
                        break;
                    case "username":
                        proceduresQuery = proceduresQuery.Where(p => p.ModifiedByUser.UserName.Contains(searchString));
                        break;
                    case "version":
                        proceduresQuery = proceduresQuery.Where(p => p.CurrentVersion.Contains(searchString));
                        break;
                }
            }


           
            int pageNumber = page ?? 1;
            var procedures = proceduresQuery.ToPagedList(pageNumber, PageSize);


            var permissions = _context.Permissions
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToPagedList(pageNumber, PageSize);
             
            return View(procedures);
        }

        [HttpGet]
        public async Task<IActionResult> SearchProcedures(string searchString, string searchOption)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var proceduresQuery = _context.Procedures.Include(p => p.ModifiedByUser).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchOption)
                {
                    case "id":
                        proceduresQuery = proceduresQuery.Where(p => p.Id.ToString().Contains(searchString));
                        break;
                    case "title":
                        proceduresQuery = proceduresQuery.Where(p => p.Title.Contains(searchString));
                        break;
                    case "date":
                        proceduresQuery = proceduresQuery.Where(p => p.CreatedAt.ToString().Contains(searchString));
                        break;
                    case "username":
                        proceduresQuery = proceduresQuery.Where(p => p.ModifiedByUser.UserName.Contains(searchString));
                        break;
                    case "version":
                        proceduresQuery = proceduresQuery.Where(p => p.CurrentVersion.Contains(searchString));
                        break;
                }
            }

            var procedures = await proceduresQuery.ToListAsync();
            return PartialView("_ProceduresTable", procedures);
        }

        [HttpGet]
        public async Task<IActionResult> AllProcedureVersions(int id, int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var procedureVersions = await _context.ProcedureVersions
                .Include(pv => pv.ModifiedByUser)
                .Where(pv => pv.ProcedureId == id)
                .OrderByDescending(pv => pv.CreatedAt)
                .ToListAsync();

            int pageNumber = page ?? 1;
            var procedures = procedureVersions.ToPagedList(pageNumber, PageSize);


            var permissions = _context.Permissions
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToPagedList(pageNumber, PageSize);
            ViewBag.ProcedureId = id;

            return View(procedures);
        }

        [HttpGet]
        [RoleAuthorize("VerVersiones")]
        public async Task<IActionResult> SearchProcedureVersions(string searchString, string searchOption)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var procedureVersionsQuery = _context.ProcedureVersions.Include(pv => pv.ModifiedByUser).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchOption)
                {
                    case "id":
                        procedureVersionsQuery = procedureVersionsQuery.Where(pv => pv.Id.ToString().Contains(searchString));
                        break;
                    case "version":
                        procedureVersionsQuery = procedureVersionsQuery.Where(pv => pv.VersionNumber.Contains(searchString));
                        break;
                    case "date":
                        if (DateTime.TryParse(searchString, out var date))
                        {
                            procedureVersionsQuery = procedureVersionsQuery.Where(pv => pv.CreatedAt.Date == date.Date);
                        }
                        break;
                    case "user":
                        procedureVersionsQuery = procedureVersionsQuery.Where(pv => pv.ModifiedByUser.UserName.Contains(searchString));
                        break;
                }
            }

            var procedureVersions = await procedureVersionsQuery.ToListAsync();
            return PartialView("_ProcedureVersionsTable", procedureVersions);
        }

        [HttpGet]
        [RoleAuthorize("CreaProc")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Crear Procedimiento";
            var model = new ProcedureViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("CreaProc")]
        public async Task<IActionResult> Create(ProcedureViewModel model, IFormFile fileUpload)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            if (fileUpload != null && fileUpload.Length > 0)
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                var filePath = Path.Combine(_procedureDirectory, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    ModelState.AddModelError("", "Ya existe un archivo con el mismo nombre.");
                    return View(model);
                }

                SaveFileToDirectory(fileUpload);

                model.Title = Path.GetFileNameWithoutExtension(fileUpload.FileName);
                model.FilePath = filePath;

                var procedure = new Procedure
                {
                    Title = model.Title,
                    FilePath = filePath,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ModifiedBy = userId,
                    CurrentVersion = "00"
                };

                _context.Procedures.Add(procedure);
                await _context.SaveChangesAsync();

                var procedureVersion = new ProcedureVersion
                {
                    ProcedureId = procedure.Id,
                    VersionNumber = procedure.CurrentVersion,
                    FilePath = procedure.FilePath,
                    CreatedAt = procedure.CreatedAt,
                    ModifiedBy = userId
                };

                _context.ProcedureVersions.Add(procedureVersion);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Debe cargar un archivo.");
            }

            return View(model);
        }


        private void SaveFileToDirectory(IFormFile fileUpload)
        {
            var fileName = Path.GetFileName(fileUpload.FileName);
            var filePath = Path.Combine(_procedureDirectory, fileName);

            if (!Directory.Exists(_procedureDirectory))
            {
                Directory.CreateDirectory(_procedureDirectory);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                fileUpload.CopyTo(stream);
            }
        }

        [HttpGet]
        [RoleAuthorize("EditaProc")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null)
            {
                return NotFound();
            }

            // Obtener la última versión del procedimiento
            var latestVersion = await _context.ProcedureVersions
                .Where(pv => pv.ProcedureId == id)
                .OrderByDescending(pv => pv.CreatedAt)
                .FirstOrDefaultAsync();

            // Si no hay versiones previas, usar el archivo del procedimiento original
            var filePath = latestVersion?.FilePath ?? procedure.FilePath;

            // Leer el contenido del archivo en bytes
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // Convertir los bytes a Base64 para enviarlo a la vista
            var base64Content = Convert.ToBase64String(fileBytes);

            // Crear el ViewModel con los datos necesarios
            var viewModel = new ProcedureViewModel
            {
                Id = procedure.Id,
                Title = procedure.Title,
                Version = latestVersion?.VersionNumber ?? procedure.CurrentVersion,
                FilePath = filePath,
                Content = base64Content,  // Aquí se asigna el contenido en Base64
                CreatedAt = procedure.CreatedAt,
                UpdatedAt = procedure.UpdatedAt
            };

            return View(viewModel);
        }


        [HttpGet]

        public async Task<IActionResult> GetDocument(int id)
        {
            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null)
            {
                return NotFound();
            }

            // Obtener la última versión del procedimiento
            var latestVersion = await _context.ProcedureVersions
                .Where(pv => pv.ProcedureId == id)
                .OrderByDescending(pv => pv.CreatedAt)
                .FirstOrDefaultAsync();

            // Si no hay versiones previas, usar el archivo del procedimiento original
            var filePath = latestVersion?.FilePath ?? procedure.FilePath;

            // Leer el contenido del archivo en bytes
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var base64Content = Convert.ToBase64String(fileBytes);

            var result = new
            {
                content = base64Content,
                fileName = $"{procedure.Title}.docx"
            };

            return Json(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("EditaProc")]
        public async Task<IActionResult> Edit(int id, ProcedureViewModel model, IFormFile fileUpload)
        {
            if (id != model.Id) return NotFound();

            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null) return NotFound();

            // Generar nuevo número de versión
            var newVersionNumber = (Int32.Parse(procedure.CurrentVersion) + 1).ToString("D2");

            // Crear nueva versión
            var newVersion = new ProcedureVersion
            {
                ProcedureId = procedure.Id,
                VersionNumber = newVersionNumber,
                FilePath = procedure.FilePath,
                CreatedAt = DateTime.Now,
                ModifiedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            // Guardar nueva versión en la base de datos
            _context.ProcedureVersions.Add(newVersion);

            // Manejar subida de archivo si es proporcionado
            if (fileUpload != null && fileUpload.Length > 0)
            {
                // Generar nombre de archivo con número de versión
                var extension = Path.GetExtension(fileUpload.FileName);
                var newFileName = $"{Path.GetFileNameWithoutExtension(fileUpload.FileName)}_v{newVersionNumber}{extension}";
                var newFilePath = Path.Combine(_procedureDirectory, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }

                newVersion.FilePath = newFilePath;  // Actualizar la ruta del archivo en la nueva versión
            }

            // Actualizar el contenido del procedimiento si es proporcionado
            if (!string.IsNullOrEmpty(model.Content))
            {
                var extension = Path.GetExtension(procedure.FilePath);
                var newFileName = $"{Path.GetFileNameWithoutExtension(procedure.FilePath)}_v{newVersionNumber}{extension}";
                var newFilePath = Path.Combine(_procedureDirectory, newFileName);

                var contentBytes = Convert.FromBase64String(model.Content);
                using (var stream = new MemoryStream(contentBytes))
                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    var document = new WordDocument();
                    document.Open(stream, Syncfusion.DocIO.FormatType.Docx);
                    document.Save(fileStream, Syncfusion.DocIO.FormatType.Docx);
                }

                newVersion.FilePath = newFilePath;  // Actualizar la ruta del archivo en la nueva versión
            }

            // Actualizar los detalles del procedimiento
            procedure.UpdatedAt = DateTime.Now;

            // No actualizar la ruta del archivo en el procedimiento principal
            _context.Update(procedure);

            // Actualizar el número de versión actual del procedimiento
            procedure.CurrentVersion = newVersion.VersionNumber;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var procedure = await _context.Procedures
                .Include(p => p.ModifiedByUser)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (procedure == null) return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(procedure.FilePath);
            var base64Content = Convert.ToBase64String(fileBytes);

            var viewModel = new ProcedureViewModel
            {
                Id = procedure.Id,
                Title = procedure.Title,
                CreatedAt = procedure.CreatedAt,
                UpdatedAt = procedure.UpdatedAt,
                FilePath = procedure.FilePath,
                Content = base64Content
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var procedure = await _context.Procedures
                .FirstOrDefaultAsync(m => m.Id == id);

            if (procedure == null) return NotFound();

            return View(procedure);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null) return NotFound();

            _context.Procedures.Remove(procedure);
            await _context.SaveChangesAsync();

            var filePath = procedure.FilePath;
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProcedureExists(int id)
        {
            return _context.Procedures.Any(e => e.Id == id);
        }
    }
}
