using Microsoft.AspNetCore.Mvc;
using SGCApp.Data;
using Microsoft.AspNetCore.Authorization;


namespace SGCApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                ViewData["Title"] = "Procedures"; // Inicializar ViewData con el título
                var model = _context.Documents.ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                // Manejar la excepción aquí, por ejemplo, registrándola o mostrando un mensaje de error
                ViewData["ErrorMessage"] = "Error al obtener los procedimientos: " + ex.Message;
                return View();
            }
        }
    }
}
