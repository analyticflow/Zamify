
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
    public class QuestionExamsController : Controller
    {
        private readonly ExamMasterDbContext _context;

        public QuestionExamsController(ExamMasterDbContext context)
        {
            _context = context;
        }

        // GET: QuestionExams
        public async Task<IActionResult> Index()
        {
            var examMasterDbContext = _context.QuestionExams.Include(q => q.Exam).Include(q => q.Question);
            return View(await examMasterDbContext.ToListAsync());
        }

        // GET: QuestionExams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionExam = await _context.QuestionExams
                .Include(q => q.Exam)
                .Include(q => q.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionExam == null)
            {
                return NotFound();
            }

            return View(questionExam);
        }

        // GET: QuestionExams/Create
        public IActionResult Create()
        {
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Id");
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id");
            return View();
        }

        // POST: QuestionExams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExamId,QuestionId,CreatedDate,UpdatedDate")] QuestionExam questionExam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questionExam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Id", questionExam.ExamId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", questionExam.QuestionId);
            return View(questionExam);
        }

        // GET: QuestionExams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionExam = await _context.QuestionExams.FindAsync(id);
            if (questionExam == null)
            {
                return NotFound();
            }
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Id", questionExam.ExamId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", questionExam.QuestionId);
            return View(questionExam);
        }

        // POST: QuestionExams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExamId,QuestionId,CreatedDate,UpdatedDate")] QuestionExam questionExam)
        {
            if (id != questionExam.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionExam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExamExists(questionExam.Id))
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
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Id", questionExam.ExamId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", questionExam.QuestionId);
            return View(questionExam);
        }

        // GET: QuestionExams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionExam = await _context.QuestionExams
                .Include(q => q.Exam)
                .Include(q => q.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionExam == null)
            {
                return NotFound();
            }

            return View(questionExam);
        }

        // POST: QuestionExams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questionExam = await _context.QuestionExams.FindAsync(id);
            if (questionExam != null)
            {
                _context.QuestionExams.Remove(questionExam);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExamExists(int id)
        {
            return _context.QuestionExams.Any(e => e.Id == id);
        }
    }
}
