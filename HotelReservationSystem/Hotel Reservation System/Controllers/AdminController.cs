using Hotel_Reservation_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DinkToPdf;
using DinkToPdf.Contracts;
using OfficeOpenXml;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Reservation_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IConverter _converter;

        public AdminController(ModelContext context, IConverter converter)
        {
            _context = context;
            _converter = converter;

        }

        public IActionResult Index()
        {
            try
            {
                int totalUsers = _context.Users.Count();
                int availableRooms = _context.Rooms.Count(r => r.Isavailable == "false");
                int bookedRooms = _context.Rooms.Count(r => r.Isavailable == "true");

                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.TotalUsers = totalUsers;
                ViewBag.AvailableRooms = availableRooms;
                ViewBag.BookedRooms = bookedRooms;
                ViewBag.numberofhotel = _context.Hotels.Count();

                ViewData["TotalUsers"] = totalUsers;
                ViewData["AvailableRooms"] = availableRooms;
                ViewData["BookedRooms"] = bookedRooms;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while fetching the data.";
                return View("Error");
            }
        }

        public IActionResult SearchReservations(DateTime startDate, DateTime endDate)
        {
            try
            {
                var reservations = _context.Reservations
                    .Where(r => r.Checkindate >= startDate && r.Checkoutdate <= endDate)
                    .ToList();

                return View(reservations);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while searching for reservations.";
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !decimal.TryParse(userIdStr, out decimal userId))
            {
                return RedirectToAction("Login", "RegistrAndLogin");
            }

            var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = _context.Users.Find(user.UserId);
                    if (existingUser == null)
                    {
                        return NotFound("User not found.");
                    }

                    // تحديث بيانات المستخدم
                    existingUser.UserName = user.UserName;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    existingUser.Password = user.Password; // تذكر أن تستخدم التشفير هنا

                    _context.Users.Update(existingUser);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Index", "Admin");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error saving changes: {ex.Message}");
                }
            }
            else
            {
                // إذا كان النموذج غير صالح، عرض الأخطاء
                ModelState.AddModelError("", "Please correct the errors in the form.");
            }

            return View(user);
        }

        public IActionResult Reports()
        {
            try
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                var monthlyRevenue = _context.Payments
                    .Where(p => p.Paymentdate.Month == currentMonth && p.Paymentdate.Year == currentYear)
                    .Sum(p => p.Amount);

                var annualRevenue = _context.Payments
                    .Where(p => p.Paymentdate.Year == currentYear)
                    .Sum(p => p.Amount);

                var monthlyReservations = _context.Reservations
                    .Where(r => r.Checkindate.Month == currentMonth && r.Checkindate.Year == currentYear)
                    .Count();

                var annualReservations = _context.Reservations
                    .Where(r => r.Checkindate.Year == currentYear)
                    .Count();

                var totalUsers = _context.Users.Count();

                ViewBag.MonthlyRevenue = monthlyRevenue;
                ViewBag.AnnualRevenue = annualRevenue;
                ViewBag.MonthlyReservations = monthlyReservations;
                ViewBag.AnnualReservations = annualReservations;
                ViewBag.TotalUsers = totalUsers;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while fetching the reports.";
                return View("Error");
            }
        }

        public IActionResult ExportReportToPdf()
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "Monthly Report"
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = "<h1>Monthly Report</h1><p>Here are the statistics for this month...</p>", // يمكنك هنا تخصيص المحتوى HTML
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
                };

                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                var file = _converter.Convert(pdf);
                return File(file, "application/pdf", "MonthlyReport.pdf");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while generating the PDF report.";
                return View("Error");
            }
        }

        public IActionResult ExportReportToExcel()
        {
            try
            {
                var payments = _context.Payments.ToList();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Payments Report");
                    worksheet.Cells["A1"].LoadFromCollection(payments, true);
                    var fileContent = package.GetAsByteArray();
                    return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PaymentsReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while generating the Excel report.";
                return View("Error");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "RegistrAndLogin"); 
        }
        

    }
}
