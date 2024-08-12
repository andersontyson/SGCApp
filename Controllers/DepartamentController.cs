using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGCApp.Attributes;
using SGCApp.Data;
using SGCApp.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


namespace SGCApp.Controllers
{
    [Authorize]
    public class DepartamentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartamentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Departament
        public async Task<IActionResult> Index()
        {
            var departaments = await _context.Departaments.ToListAsync();
            return View(departaments);
        }

        // GET: Departament/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Departament departament = _context.Departaments.Find(id);
            if (departament == null)
            {
                return NotFound();
            }
            return View(departament);
        }

        // GET: Departament/Create
        [RoleAuthorize("CreaDep")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departament/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("CreaDep")]
        public IActionResult Create([Bind("Id,Name")] Departament departament)
        {
            if (ModelState.IsValid)
            {
                _context.Departaments.Add(departament);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(departament);
        }

        // GET: Departament/Edit/5
        [RoleAuthorize("EditaDep")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Departament departament = _context.Departaments.Find(id);
            if (departament == null)
            {
                return NotFound();
            }
            return View(departament);
        }

        // POST: Departament/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("EditaDep")]
        public IActionResult Edit([Bind("Id,Name")] Departament departament)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(departament).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(departament);
        }

        // GET: Departament/Delete/5
        [RoleAuthorize("BorraDep")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Departament departament = _context.Departaments.Find(id);
            if (departament == null)
            {
                return NotFound();
            }
            return View(departament);
        }

        // POST: Departament/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("BorraDep")]
        public IActionResult DeleteConfirmed(int id)
        {
            Departament departament = _context.Departaments.Find(id);
            if (departament == null)
            {
                return NotFound();
            }
            _context.Departaments.Remove(departament);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
