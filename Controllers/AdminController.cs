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

namespace Zamify.Controllers
{
    public class AdminController : Controller
    {
        private readonly ExamMasterDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(ExamMasterDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            try
            {
                if (HttpContext.Session.GetString("UserRole") != "Admin")
                {
                    return RedirectToAction("Login", "Home");
                }
                return View(await _context.Teachers.ToListAsync());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Admin/Index: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (teacher == null)
                {
                    return NotFound();
                }

                return View(teacher);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Admin/Details: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Admin/Create GET: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password")] Teacher teacher)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check for existing email
                    if (await _context.Teachers.AnyAsync(t => t.Email == teacher.Email))
                    {
                        ModelState.AddModelError("Email", "This email address is already registered.");
                        return View(teacher);
                    }

                    teacher.CreatedDate = DateTime.Now;
                    teacher.UpdatedDate = DateTime.Now;
                    _context.Add(teacher);
                    await _context.SaveChangesAsync();

                    try
                    {
                        await SendWelcomeEmail(teacher.Email, teacher.Name, teacher.Password);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Email sending failed: {ex.Message}");
                        TempData["EmailError"] = "Teacher created, but welcome email failed to send.";
                    }

                    return RedirectToAction(nameof(Index));
                }
                return View(teacher);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Admin/Create POST: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null)
                {
                    return NotFound();
                }
                return View(teacher);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Admin/Edit GET: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password")] Teacher teacher)
        {
            try
            {
                if (id != teacher.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var existingTeacher = await _context.Teachers.FindAsync(id);
                        if (existingTeacher == null)
                        {
                            return NotFound();
                        }

                        // Check for email conflict with other teachers
                        if (await _context.Teachers.AnyAsync(t => t.Email == teacher.Email && t.Id != id))
                        {
                            ModelState.AddModelError("Email", "This email address is already registered to another teacher.");
                            return View(teacher);
                        }

                        existingTeacher.Name = teacher.Name;
                        existingTeacher.Email = teacher.Email;
                        existingTeacher.Password = teacher.Password;
                        existingTeacher.UpdatedDate = DateTime.Now;

                        _context.Update(existingTeacher);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TeacherExists(teacher.Id))
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
                return View(teacher);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Admin/Edit POST: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (teacher == null)
                {
                    return NotFound();
                }

                return View(teacher);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Admin/Delete GET: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null)
                {
                    return NotFound();
                }

                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();

                try
                {
                    await SendDeletionEmail(teacher.Email, teacher.Name);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Deletion email failed: {ex.Message}");
                    TempData["DeletionEmailError"] = "Teacher deleted, but notification email failed to send.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Admin/Delete POST: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }

        private async Task SendWelcomeEmail(string email, string name, string password)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Admin", _configuration["EmailSettings:SmtpUser"]));
            message.To.Add(new MailboxAddress(name, email));
            message.Subject = "Welcome to the System";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Hello {name},\n\nYour account has been successfully created.\n\nEmail: {email}\nPassword: {password}\n\nLogin at [Your Portal Link].\n\nBest Regards,\nAdmin Team"
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
        }

        private async Task SendDeletionEmail(string email, string name)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Admin", _configuration["EmailSettings:SmtpUser"]));
            message.To.Add(new MailboxAddress(name, email));
            message.Subject = "Account Deletion Notification";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Dear {name},\n\nWe regret to inform you that your teacher account has been removed from our system.\n\nIf you believe this was done in error, please contact the administrator.\n\nBest Regards,\nAdmin Team"
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
        }
    }
}