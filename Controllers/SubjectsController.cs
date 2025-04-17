using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zamify.Models;

namespace Zamify.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly ExamMasterDbContext _context;

        public SubjectsController(ExamMasterDbContext context)
        {
            _context = context;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Subjects.ToListAsync());
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Subject subject)
        {
            // Trim whitespace from the subject name
            subject.Name = subject.Name?.Trim();

            if (ModelState.IsValid)
            {
                // Server-side duplicate check (normalized)
                var normalizedName = subject.Name.ToLower();
                if (await _context.Subjects.AnyAsync(s => s.Name.Trim().ToLower() == normalizedName))
                {
                    ModelState.AddModelError("Name", "Subject already exists.");
                    return View(subject);
                }

                subject.CreatedDate = DateTime.Now;
                subject.UpdatedDate = DateTime.Now;
                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }


        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreatedDate")] Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            // Trim whitespace from the subject name
            subject.Name = subject.Name?.Trim();

            if (ModelState.IsValid)
            {
                // Server-side duplicate check (normalized, exclude current)
                var normalizedName = subject.Name.ToLower();
                if (await _context.Subjects.AnyAsync(s =>
                    s.Name.Trim().ToLower() == normalizedName &&
                    s.Id != id))
                {
                    ModelState.AddModelError("Name", "Subject already exists.");
                    return View(subject);
                }

                try
                {
                    subject.UpdatedDate = DateTime.Now;
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.Id))
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
            return View(subject);
        }



        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }

        //FOR SUBJECT VERIFICATION 

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifySubject(string name, int? id)
        {
            if (string.IsNullOrEmpty(name))
                return Json(true); // Skip validation if empty

            // Normalize input
            var normalizedName = name.Trim().ToLower();

            // Check for existing subjects (case-insensitive, trimmed)
            var existingSubject = await _context.Subjects
                .FirstOrDefaultAsync(s =>
                    s.Name.Trim().ToLower() == normalizedName &&
                    s.Id != id
                );

            return existingSubject != null
                ? Json($"Subject '{name}' already exists.")
                : Json(true);
        }
    }
}
