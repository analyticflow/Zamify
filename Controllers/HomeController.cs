using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zamify.Models;
using Microsoft.AspNetCore.Http;

namespace Zamify.Controllers
{
    public class HomeController : Controller
    {
        private readonly ExamMasterDbContext context;

        //private readonly OnlineExamDbContext context;
        //public HomeController(OnlineExamDbContext context)
        //{
        //    this.context = context;
        //}

        public HomeController(ExamMasterDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if user exists in Admin table
                    var admin = context.Admins.SingleOrDefault(a => a.Email == model.Email && a.Password == model.Password);
                    //var admin = context.Admins.Where(x=>x.Email == model.Email && x.Password == model.Password).FirstOrDefault();
                    if (admin != null)
                    {
                        HttpContext.Session.SetString("UserRole", "Admin");
                        //  HttpContext.Session.SetString("UserRole", admin.Email);
                        return RedirectToAction("Index", "Admin");
                    }

                    // Check if user exists in Teacher table
                    var teacher = context.Teachers.SingleOrDefault(t => t.Email == model.Email && t.Password == model.Password);
                    if (teacher != null)
                    {
                        HttpContext.Session.SetString("UserRole", "Teacher");
                        HttpContext.Session.SetInt32("TeacherId", teacher.Id); // Store TeacherId in session
                                                                               // HttpContext.Session.SetString("TeacherId", teacher.Id.ToString());

                      //  return RedirectToAction("Index", "Teacher");
                        return RedirectToAction("Index", "Subjects");
                    }

                    // Check if user exists in Student table
                    var student = context.Students.SingleOrDefault(s => s.Email == model.Email && s.Password == model.Password);
                    if (student != null)
                    {
                        HttpContext.Session.SetString("UserRole", "Student");
                        return RedirectToAction("SDashboard", "Student");
                    }

                    // Unified error for invalid login
                    ModelState.AddModelError("", "Invalid email or password. Please try again.");
                    //else
                    //{
                    //    ViewBag.Message = "Login Failed ...";
                    //}
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Oops! Something went wrong. Please try again later.");
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            if(HttpContext.Session.GetString("UserRole")!= null)
            {
                HttpContext.Session.Remove("UserRole");
              //  return RedirectToAction("Index", "Home");
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
