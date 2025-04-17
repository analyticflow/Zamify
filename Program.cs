//using Microsoft.EntityFrameworkCore;
//using Zamify.Models;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//// build Session
//builder.Services.AddSession();

//// Adding Connection 
//var provider = builder.Services.BuildServiceProvider();
//var config = provider.GetRequiredService<IConfiguration>();
//builder.Services.AddDbContext<ExamMasterDbContext>(item => item.UseSqlServer(config.GetConnectionString("dbcs")));


//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}
////using session 
//app.UseSession();
//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//   pattern: "{controller=Home}/{action=Login}/{id?}");
//   // pattern: "{controller=Home}/{action=Index}/{id?}");
//   // pattern: "{controller=Questions}/{action=Index}/{id?}");
//    //pattern: "{controller=Exams}/{action=Index}/{id?}");
//   //pattern: "{controller=Admin}/{action=ADashboard}/{id?}");
//   //pattern: "{controller=Teacher}/{action=TDashboard}/{id?}");
//   //pattern: "{controller=Student}/{action=SDashboard}/{id?}");

//app.Run();



using Microsoft.EntityFrameworkCore;
using Zamify.Models;

var builder = WebApplication.CreateBuilder(args);

// ? Add Session & TempData support
builder.Services.AddControllersWithViews()
       .AddSessionStateTempDataProvider(); // ?? Required for TempData

builder.Services.AddSession(); // ?? Required for Session-based TempData

// ? Add DBContext with SQL Server
var provider = builder.Services.BuildServiceProvider();
var config = provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<ExamMasterDbContext>(item =>
    item.UseSqlServer(config.GetConnectionString("dbcs")));

var app = builder.Build();

// ? Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession(); // ?? Must come before routing/mvc
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization(); // Only if you use [Authorize]

// ? Route setup
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
