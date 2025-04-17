using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zamify.Models;
using System.Diagnostics;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;


namespace Zamify.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ExamMasterDbContext _context;
        private readonly IConfiguration _configuration;

        public TeacherController(ExamMasterDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Teacher")
                {
                    return RedirectToAction("Login", "Home");
                }
                return View(await _context.Students.ToListAsync());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Teacher/Index: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Teacher/Create GET: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password")] Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student); // Automatically shows validation errors from model
            }

            // Check for existing email
            if (await _context.Students.AnyAsync(s => s.Email == student.Email))
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View(student);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Try to send email FIRST
                if (!await TrySendStudentWelcomeEmail(student.Email, student.Name, student.Password))
                {
                    ModelState.AddModelError(string.Empty, "Registration failed - invalid email address");
                    return View(student);
                }

                // Only save if email succeeded
                student.CreatedDate = DateTime.Now;
                student.UpdatedDate = DateTime.Now;
                _context.Add(student);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Error creating student. Please try again.");
                return View(student);
            }
        }

        private async Task<bool> TrySendStudentWelcomeEmail(string email, string name, string password)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Teacher Portal", _configuration["EmailSettings:SmtpUser"]));
                message.To.Add(new MailboxAddress(name, email));
                message.Subject = "Student Account Created";

                var bodyBuilder = new BodyBuilder
                {
                    TextBody = $"Hello {name},\n\nYour student account has been created by your teacher.\n\nEmail: {email}\nPassword: {password}\n\nLogin at [Student Portal Link].\n\nBest Regards,\nSchool Team"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]),
                    MailKit.Security.SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(_configuration["EmailSettings:SmtpUser"], _configuration["EmailSettings:SmtpPass"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var student = await _context.Students.FindAsync(id);
                if (student == null) return NotFound();

                return View(student);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Teacher/Edit GET: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password")] Student student)
        {
            try
            {
                if (id != student.Id) return NotFound();

                if (ModelState.IsValid)
                {
                    var existingStudent = await _context.Students.FindAsync(id);
                    if (existingStudent == null) return NotFound();

                    if (await _context.Students.AnyAsync(s => s.Email == student.Email && s.Id != id))
                    {
                        ModelState.AddModelError("Email", "This email is already registered.");
                        return View(student);
                    }

                    existingStudent.Name = student.Name;
                    existingStudent.Email = student.Email;
                    existingStudent.Password = student.Password;
                    existingStudent.UpdatedDate = DateTime.Now;

                    _context.Update(existingStudent);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(student);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Teacher/Edit POST: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Id == id);

                return student == null ? NotFound() : View(student);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Teacher/Details: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Id == id);

                return student == null ? NotFound() : View(student);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Teacher/Delete GET: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null) return NotFound();

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Teacher/Delete POST: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        //private async Task SendStudentWelcomeEmail(string email, string name, string password)
        //{
        //    var message = new MimeMessage();
        //    message.From.Add(new MailboxAddress("Teacher Portal", _configuration["EmailSettings:SmtpUser"]));
        //    message.To.Add(new MailboxAddress(name, email));
        //    message.Subject = "Student Account Created";

        //    var bodyBuilder = new BodyBuilder
        //    {
        //        TextBody = $"Hello {name},\n\nYour student account has been created by your teacher.\n\nEmail: {email}\nPassword: {password}\n\nLogin at [Student Portal Link].\n\nBest Regards,\nSchool Team"
        //    };

        //    message.Body = bodyBuilder.ToMessageBody();

        //    using var client = new SmtpClient();
        //    await client.ConnectAsync(
        //        _configuration["EmailSettings:SmtpServer"],
        //        int.Parse(_configuration["EmailSettings:SmtpPort"]),
        //        MailKit.Security.SecureSocketOptions.StartTls);
        //    await client.AuthenticateAsync(_configuration["EmailSettings:SmtpUser"], _configuration["EmailSettings:SmtpPass"]);
        //    await client.SendAsync(message);
        //    await client.DisconnectAsync(true);
        //}
    }
}