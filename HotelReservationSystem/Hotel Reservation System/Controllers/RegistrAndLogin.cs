using Hotel_Reservation_System.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Hotel_Reservation_System.Controllers
{
    public class RegistrAndLogin : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public RegistrAndLogin(ModelContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName, Password, Email, Phone, UserRole")] User user)
        {
            if (ModelState.IsValid)
            {
                // التحقق من وجود اسم المستخدم مسبقًا
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == user.UserName);

                if (existingUser == null)
                {
                    // إضافة المستخدم إلى قاعدة البيانات
                    _context.Add(user);
                    await _context.SaveChangesAsync();

                    // إعادة التوجيه إلى صفحة تسجيل الدخول
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "اسم المستخدم موجود مسبقًا.");
                }
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);

                if (user != null)
                {
                    // تخزين بيانات المستخدم في الجلسة
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("UserRole", user.UserRole);
                    ViewBag.UserId = HttpContext.Session.GetString("UserId");

                    ViewBag.UserName = user.UserName;

                    // توجيه المستخدم بناءً على دوره
                    if (user.UserRole == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        // التوجيه إلى الصفحة التي كان عليها المستخدم
                        string returnUrl = HttpContext.Request.Query["ReturnUrl"];
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "اسم المستخدم أو كلمة المرور غير صحيحة.");
                }
            }

            return View();
        }

        public IActionResult Logout()
        {
            // إفراغ الجلسة وتوجيه المستخدم إلى صفحة تسجيل الدخول
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public IActionResult Guest()
        {
            // قم بتسجيل الدخول كزائر، أو قم بإعداد الجلسة كزائر
            // ...

            return RedirectToAction("Index", "Home");
        }
        public IActionResult SomePage()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            var userRole = HttpContext.Session.GetString("UserRole");

            // تحقق من البيانات وتأكد من أنها صحيحة
            if (userId != null && userName != null && userRole != null)
            {
                // قم بما يلزم بناءً على بيانات الجلسة
            }
            else
            {
                // إعادة التوجيه إلى صفحة تسجيل الدخول
                return RedirectToAction("Login", "RegistrAndLogin");
            }

            return View();
        }

    }

}
