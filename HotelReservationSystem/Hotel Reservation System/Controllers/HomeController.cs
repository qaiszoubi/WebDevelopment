using Hotel_Reservation_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using email_sender;
using Newtonsoft.Json;

namespace Hotel_Reservation_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ModelContext _context;
        private readonly ILogger<HomeController> _logger;
        private static List<ContactMessage> _contactMessages = new List<ContactMessage>();
        private readonly IEmailSender _emailSender;
        private readonly PdfService _pdfService;


        public HomeController(ILogger<HomeController> logger, ModelContext context, PdfService pdfService, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _pdfService = pdfService;

        }

        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ContactUs(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                // إعداد الرسالة مع معرف فريد
                var contactMessage = new ContactMessage
                {
                    Id = new Random().Next(1, 1000), // إنشاء معرف عشوائي كمثال
                    Name = model.Name,
                    Email = model.Email,
                    Message = model.Message,
                    SentDate = DateTime.Now
                };

                // إضافة الرسالة إلى قائمة الرسائل
                _contactMessages.Add(contactMessage);

                // رسالة تأكيد للمستخدم
                TempData["SuccessMessage"] = "Your message has been sent successfully!";

                // إعادة توجيه إلى صفحة "اتصل بنا"
                return RedirectToAction("ContactUs");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Messages()
        {
            // عرض الرسائل في عرض صفحة "الرسائل"
            return View(_contactMessages);
        }
    
    public IActionResult About()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError("An error occurred while processing your request.");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Hotel()
        {
            var hotels = _context.Hotels.ToList();
            return View(hotels);
        }

        public IActionResult Room(decimal hotelid)
        {
            var rooms = _context.Rooms.Where(r => r.Hotelid == hotelid).ToList();
            return View(rooms);
        }

        [HttpGet]
        public IActionResult Payment(int reservationId)
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var payment = _context.Payments.FirstOrDefault(p => p.Reservationid == reservationId);

            if (payment == null)
            {
                var newPayment = new Payment
                {
                    Reservationid = reservationId
                };

                ViewBag.Message = "No payment details available for this reservation.";
                return View(newPayment);
            }

            return View(payment);
        }

        [HttpPost]
        public IActionResult Payment(Payment model)
        {
            if (ModelState.IsValid)
            {
                var existingPayment = _context.Payments.FirstOrDefault(p => p.Reservationid == model.Reservationid);

                if (existingPayment == null)
                {
                    _context.Payments.Add(model);
                    _context.SaveChanges();

                    return RedirectToAction("PaymentSuccess", new { reservationId = model.Reservationid });
                }
                else
                {
                    ModelState.AddModelError("", "Payment already exists for this reservation.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Reservation(int roomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Roomid == roomId);

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            if (room.Isavailable == "true")
            {
                TempData["ErrorMessage"] = "Sorry, this room is currently reserved.";
                return RedirectToAction("Room");
            }

            var reservationModel = new Reservation
            {
                Room = room,
                Roomid = room.Roomid
            };

            return View(reservationModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservation(Reservation model)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Roomid == model.Roomid);

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            if (room.Isavailable == "true")
            {
                TempData["ErrorMessage"] = "Sorry, this room has just been reserved by another user.";
                return RedirectToAction("Room");
            }

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User is not logged in.";
                return RedirectToAction("LogIn", "RegistrAndLogin");
            }

            model.UserId = decimal.Parse(userId);

            room.Isavailable = "true";
            _context.Reservations.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Payment", new { reservationId = model.Reservationid });
        }

        [HttpGet]
        public IActionResult PaymentSuccess()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PaymentSuccess(string parameter)
        {
            var recentPayment = await _context.Payments
                .Include(p => p.Reservation) 
                .ThenInclude(r => r.Room)
                .OrderByDescending(p => p.Paymentid) 
                .FirstOrDefaultAsync(); 

            if (recentPayment == null || recentPayment.Reservation == null || recentPayment.Reservation.Room == null)
            {
                return NotFound(); 
            }

            var roomNumber = recentPayment.Reservation.Room.Roomnumber; 
            var amountPaid = recentPayment.Amount; 

            var projectName = "Hotel Reservation System"; 
            var subject = projectName;
            var htmlContent = $"<h1>Invoice</h1><p>Room Number: {roomNumber}</p><p>Amount Paid: {amountPaid:C}</p>"; 

            var fixedEmail = "alzoubiqais0@gmail.com";

            await _emailSender.SendEmailAsync(fixedEmail, subject, htmlContent);

            TempData["SuccessMessage"] = "Invoice has been sent successfully.";

            return View("PaymentSuccess"); 
        }


        private Reservation? GetReservationDetails(decimal reservationId)
        {
            return _context.Reservations
                           .Include(r => r.Room)
                           .FirstOrDefault(r => r.Reservationid == reservationId);
        }
        [HttpGet]
        public IActionResult UserEditProfile()
        {
            // Retrieve user data and pass it to the view, if needed
            return View();
        }

        [HttpPost]
        public IActionResult UserEditProfile(User user)
        {
            if (ModelState.IsValid)
            {
                // تحديث معلومات المستخدم في قاعدة البيانات
                _context.Users.Update(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Index", "Home");
            }

            return View(user); // إعادة عرض النموذج مع الأخطاء إذا كانت موجودة
        }

    }
}
