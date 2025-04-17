using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zamify.Models;

namespace Zamify.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ExamMasterDbContext _context;

        public QuestionsController(ExamMasterDbContext context)
        {
            _context = context;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            var examMasterDbContext = _context.Questions.Include(q => q.Subject);
            return View(await examMasterDbContext.ToListAsync());
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Questions/Create
        public IActionResult Create()
        {
            // ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,SubjectId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption,CreatedDate,UpdatedDate")] Question question)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(question);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", question.SubjectId);
        //    return View(question);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption")] Question question)
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", question.SubjectId);
                return View(question);
            }

            question.CreatedDate = DateTime.Now;
            question.UpdatedDate = DateTime.Now;
            _context.Add(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", question.SubjectId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption,CreatedDate,UpdatedDate")] Question question)
        //{
        //    if (id != question.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(question);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!QuestionExists(question.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", question.SubjectId);
        //    return View(question);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            var existingQuestion = await _context.Questions.FindAsync(id);
            if (existingQuestion == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update fields from the input model
                    existingQuestion.SubjectId = question.SubjectId;
                    existingQuestion.QuestionText = question.QuestionText;
                    existingQuestion.OptionA = question.OptionA;
                    existingQuestion.OptionB = question.OptionB;
                    existingQuestion.OptionC = question.OptionC;
                    existingQuestion.OptionD = question.OptionD;
                    existingQuestion.CorrectOption = question.CorrectOption;
                    existingQuestion.UpdatedDate = DateTime.Now; // Auto-update

                    _context.Update(existingQuestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
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
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", question.SubjectId);
            return View(question);
        }


        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }


        // GET: Questions/CreateBulk
        public IActionResult CreateBulk()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
            return View(new List<Question> { new Question() }); // Start with one empty question
        }

        // POST: Questions/CreateBulk
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBulk(int SubjectId, List<Question> questions)
        {
            if (ModelState.IsValid)
            {
                foreach (var question in questions)
                {
                    if (!string.IsNullOrEmpty(question.QuestionText))
                    {
                        question.SubjectId = SubjectId;
                        question.CreatedDate = DateTime.Now;
                        question.UpdatedDate = DateTime.Now;
                        _context.Add(question);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", SubjectId);
            return View(questions);
        }


        // GetOptions Method to create exam 
        [HttpGet]
        public async Task<IActionResult> GetOptions(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Subject)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return Content("<p>Question not found</p>");
            }

            var optionsHtml = new StringBuilder();
            optionsHtml.Append($"<p><strong>Question:</strong> {question.QuestionText}</p>");
            optionsHtml.Append("<ul class='list-group'>");

            // Add each option with indication of correct answer
            optionsHtml.Append($"<li class='list-group-item {(question.CorrectOption == "A" ? "list-group-item-success" : "")}'>A. {question.OptionA}</li>");
            optionsHtml.Append($"<li class='list-group-item {(question.CorrectOption == "B" ? "list-group-item-success" : "")}'>B. {question.OptionB}</li>");
            optionsHtml.Append($"<li class='list-group-item {(question.CorrectOption == "C" ? "list-group-item-success" : "")}'>C. {question.OptionC}</li>");
            optionsHtml.Append($"<li class='list-group-item {(question.CorrectOption == "D" ? "list-group-item-success" : "")}'>D. {question.OptionD}</li>");

            optionsHtml.Append("</ul>");

            return Content(optionsHtml.ToString());
        }
    }
}
