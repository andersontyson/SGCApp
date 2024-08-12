using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGCApp.Models;

namespace SGCApp.Views.Document
{
    public class EditExcelModel : PageModel
    {
        [BindProperty]
        public DocumentViewModel Document { get; set; }

        public void OnGet()
        {
        }
    }
}
