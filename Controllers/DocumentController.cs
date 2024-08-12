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
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _documentDirectory = "wwwroot/documents";
        private const int PageSize = 8;

        public DocumentController(ApplicationDbContext context)
        {
            _context = context;
        }

      
        public async Task<IActionResult> Index(string searchString, int? page, string searchOption)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized();
                }

                var user = await _context.Users
                    .Include(u => u.Departament)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || user.Departament == null)
                {
                    return NotFound("Usuario no encontrado o sin departamento asignado.");
                }

                var departamentName = user.Departament.Name;

                var documentsQuery = _context.Documents.AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    switch (searchOption)
                    {
                        case "id":
                            documentsQuery = documentsQuery.Where(d => d.Id.ToString().Contains(searchString));
                            break;
                        case "title":
                            documentsQuery = documentsQuery.Where(d => d.Title.Contains(searchString));
                            break;
                        case "date":
                            documentsQuery = documentsQuery.Where(d => d.CreatedAt.ToString().Contains(searchString));
                            break;
                    }
                }

                var documents = await documentsQuery
                    .Where(d => d.FilePath.Contains(departamentName))
                    .ToListAsync();



                int pageNumber = page ?? 1;
                var documentp = documentsQuery.ToPagedList(pageNumber, PageSize);
                ViewData["CurrentFilter"] = searchString;
                ViewData["SearchOption"] = searchOption;



                return View(documentp);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchDocuments(string searchString, string searchOption)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(u => u.Departament)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Departament == null)
            {
                return NotFound("Usuario no encontrado o sin departamento asignado.");
            }

            var departamentName = user.Departament.Name;

            var documentsQuery = _context.Documents.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchOption)
                {
                    case "id":
                        documentsQuery = documentsQuery.Where(d => d.Id.ToString().Contains(searchString));
                        break;
                    case "title":
                        documentsQuery = documentsQuery.Where(d => d.Title.Contains(searchString));
                        break;
                    case "date":
                        documentsQuery = documentsQuery.Where(d => d.CreatedAt.ToString().Contains(searchString));
                        break;
                }
            }

            var documents = await documentsQuery
                .Where(d => d.FilePath.Contains(departamentName))
                .ToListAsync();

            return PartialView("_DocumentsTable", documents);
        }


        [RoleAuthorize("CreaDoc")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Crear Documento";
            var model = new DocumentViewModel();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("CreaDoc")]
        public async Task<IActionResult> Create(DocumentViewModel model, IFormFile fileUpload)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized();
                }

                var user = await _context.Users
                    .Include(u => u.Departament)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || user.Departament == null)
                {
                    return NotFound("Usuario no encontrado o sin departamento asignado.");
                }

                if (fileUpload != null && fileUpload.Length > 0)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var departamentName = user.Departament.Name;
                    var filePath = Path.Combine(_documentDirectory, departamentName, fileName);

                    // Verificar si ya existe un archivo con el mismo nombre en el directorio del departamento
                    if (System.IO.File.Exists(filePath))
                    {
                        ModelState.AddModelError("", "Ya existe un archivo con el mismo nombre.");
                        return View(model);
                    }

                    // Guardar el archivo
                    SaveFileToUserDirectory(fileUpload, departamentName);

                    model.Title = Path.GetFileNameWithoutExtension(fileUpload.FileName);
                    model.FilePath = filePath;

                    var document = new Document
                    {
                        Title = model.Title,
                        FilePath = filePath,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        ModifiedBy = userId,
                        CurrentVersion = "00"
                    };

                    _context.Documents.Add(document);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Debe cargar un archivo.");
                }
            }
            else
            {
                return Unauthorized();
            }

            return View(model);
        }

        private void SaveFileToUserDirectory(IFormFile fileUpload, string departamentName)
        {
            var departamentDirectory = Path.Combine(_documentDirectory, departamentName);
            var fileName = Path.GetFileName(fileUpload.FileName);
            var filePath = Path.Combine(departamentDirectory, fileName);

            if (!Directory.Exists(departamentDirectory))
            {
                Directory.CreateDirectory(departamentDirectory);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                fileUpload.CopyTo(stream);
            }
        }

        [HttpGet]
        [RoleAuthorize("EditaDoc")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            // Obtener la última versión del procedimiento
            var latestVersion = await _context.DocumentVersions
                .Where(pv => pv.DocumentId == id)
                .OrderByDescending(pv => pv.CreatedAt)
                .FirstOrDefaultAsync();

            // Si no hay versiones previas, usar el archivo del procedimiento original
            var filePath = latestVersion?.FilePath ?? document.FilePath;

            // Leer el contenido del archivo en bytes
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // Convertir los bytes a Base64 para enviarlo a la vista
            var base64Content = Convert.ToBase64String(fileBytes);

            // Crear el ViewModel con los datos necesarios
            var viewModel = new DocumentViewModel
            {
                Id = document.Id,
                Title = document.Title,
                Version = latestVersion?.VersionNumber ?? document.CurrentVersion,
                FilePath = filePath,
                Content = base64Content,  // Aquí se asigna el contenido en Base64
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            // Obtener la última versión del documento
            var latestVersion = await _context.DocumentVersions
                .Where(pv => pv.DocumentId == id)
                .OrderByDescending(pv => pv.CreatedAt)
                .FirstOrDefaultAsync();

            // Si no hay versiones previas, usar el archivo del procedimiento original
            var filePath = latestVersion?.FilePath ?? document.FilePath;

            // Leer el contenido del archivo en bytes
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var base64Content = Convert.ToBase64String(fileBytes);

            var result = new
            {
                content = base64Content,
                fileName = $"{document.Title}.docx"
            };

            return Json(result);
        }

        private async Task<IActionResult> DownloadExcelFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var memory = new MemoryStream();

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("EditaDoc")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, DocumentViewModel model, IFormFile fileUpload)
        {
            if (id != model.Id) return NotFound();

            var document = await _context.Documents.FindAsync(id);
            if (document == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.Departament)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Departament == null)
            {
                return NotFound("Usuario no encontrado o sin departamento asociado.");
            }

            if (!document.FilePath.Contains(user.Departament.Name))
            {
                return Unauthorized();
            }

            // Generar nuevo número de versión
            var newVersionNumber = (Int32.Parse(document.CurrentVersion) + 1).ToString("D2");

            // Crear nueva versión
            var newVersion = new Models.DocumentVersion
            {
                DocumentId = document.Id,
                VersionNumber = newVersionNumber,
                FilePath = document.FilePath,
                CreatedAt = DateTime.Now, // Asignar directamente DateTime.Now sin convertir a string
                ModifiedBy = userId
            };


            // Guardar nueva versión en la base de datos
            _context.DocumentVersions.Add(newVersion);

            // Manejar subida de archivo si es proporcionado
            if (fileUpload != null && fileUpload.Length > 0)
            {
                // Generar nombre de archivo con número de versión
                var extension = Path.GetExtension(fileUpload.FileName);
                var newFileName = $"{Path.GetFileNameWithoutExtension(fileUpload.FileName)}_v{newVersionNumber}{extension}";
                var newFilePath = Path.Combine(_documentDirectory, user.Departament.Name, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }

                newVersion.FilePath = newFilePath;  // Actualizar la ruta del archivo en la nueva versión
            }

            // Actualizar el contenido del procedimiento si es proporcionado
            if (!string.IsNullOrEmpty(model.Content))
            {
                var extension = Path.GetExtension(document.FilePath);
                var newFileName = $"{Path.GetFileNameWithoutExtension(document.FilePath)}_v{newVersionNumber}{extension}";
                var newFilePath = Path.Combine(_documentDirectory, user.Departament.Name, newFileName);

                var contentBytes = Convert.FromBase64String(model.Content);
                using (var stream = new MemoryStream(contentBytes))
                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    var documentx = new WordDocument();
                    documentx.Open(stream, Syncfusion.DocIO.FormatType.Docx);
                    documentx.Save(fileStream, Syncfusion.DocIO.FormatType.Docx);
                }

                newVersion.FilePath = newFilePath;  // Actualizar la ruta del archivo en la nueva versión
            }

            // Actualizar los detalles del procedimiento
            document.UpdatedAt = DateTime.Now;

            // No actualizar la ruta del archivo en el procedimiento principal
            _context.Update(document);

            // Actualizar el número de versión actual del procedimiento
            document.CurrentVersion = newVersion.VersionNumber;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllDocumentVersions(int id, int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var documentVersions = await _context.DocumentVersions
                .Include(pv => pv.ModifiedByUser)
                .Where(pv => pv.DocumentId == id)
                .OrderByDescending(pv => pv.CreatedAt)
                .ToListAsync();
                int pageNumber = page ?? 1;
                var documents = documentVersions.ToPagedList(pageNumber, PageSize);


                var permissions = _context.Permissions
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Name)
                    .ToPagedList(pageNumber, PageSize);
                ViewBag.DocumentId = id;

                return View(documents);
        }


    }
}
