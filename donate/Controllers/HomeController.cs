using donate.data;
using donate.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Azure.Core;
using System.Linq;
using Microsoft.JSInterop.Infrastructure;
using donate.Helper;

namespace donate.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        int requestCount;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            
            _logger = logger;
            _context = context;
        }
        public IActionResult username()
        {
            return View();  
        }
            public IActionResult Requests()
        {


            bool isUser = SessionHelper.IsUser(HttpContext);
            if (!isUser)
                return RedirectToAction("Login", "Home");

            var currentUserId = SessionHelper.GetCurrentUserId(HttpContext);
            var userType = SessionHelper.GetUserType(HttpContext);
            

            if (userType == "Beneficiary")
            {
                var requests = _context.Requests
                    .Where(x => x.UserId == currentUserId)
                    .Include(x => x.Donation)
                    .ThenInclude(d => d.User) // Include donor information
                    .ToList();
                return View(requests);
            }
            else if (userType == "Donor")
            {
                var requests = _context.Requests
                    .Include(x => x.Donation)
                    .ThenInclude(d => d.User)
                    .Where(r => (r.Donation.UserId == currentUserId) && (r.RequestStatus == "Pending"))
                    .Include(r => r.User) // Include requester information
                    .ToList();
                return View(requests);
            }

            return RedirectToAction("Index", "Home");
        }





        public IActionResult CancelRequest(int id)
        {
            var req =_context.Requests
                .Include(x=>x.Donation)
                .Where(r=>r.Id == id).FirstOrDefault();
            if(req != null &&req.Donation != null) { 
            req.Donation.DonationStatus = "Available";
            _context.Remove(req);
                _context.SaveChanges();
            }
            return RedirectToAction("Requests", "Home");
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult about() {
            return View();

        }
        
       

        public IActionResult donate()
        {
            bool isUser = SessionHelper.IsUser(HttpContext);
            if (!isUser )
                return RedirectToAction("Login", "Home");
            var categories = _context.Categorys.ToList();
            var currentUserId = SessionHelper.GetCurrentUserId(HttpContext);
            requestCount = _context.Requests
               .Where((r => (r.Donation.UserId == currentUserId) && (r.RequestStatus == "Pending")))
                .Count();
            
            var userName = SessionHelper.GetUserName(HttpContext);
            
            var userInitial = userName.Substring(0, 1);  // الحصول على أول حرف من الاسم
            ViewData["UserName"] = userName;
            ViewData["UserInitial"] = userInitial;
            
            @ViewBag.RequestCount = requestCount;
            return View(categories);
        }

       
        public IActionResult Emergency()
        {
            bool isUser = SessionHelper.IsUser(HttpContext);
            if (!isUser)
                return RedirectToAction("Login", "Home");
            return View();
        }
        public IActionResult Donations(int categoryId)
        {
            bool isUser = SessionHelper.IsUser(HttpContext);
            if (!isUser)
                return RedirectToAction("Login", "Home");
            var donations = _context.Donations
                .Include(d => d.User)
                .Include(c => c.Category)
                .Where(d => d.CategoryId == categoryId && d.DonationStatus == "Available")
                .ToList();
            if (!donations.Any())
            {
                ViewBag.Message = "لا يوجد أي تبرعات متاحة.";
            }
            else
            {
                ViewBag.Message = null;
            }
            ViewBag.CategoryId = categoryId;

            return View(donations);
        }

        

        [HttpPost]
        public async Task<IActionResult> AddDonation(Donation donation, IFormFile Image)
        {
            if (Image != null && Image.Length > 0)
            {
                // تحديد مسار الحفظ
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/donationImages");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Image.FileName);

                // التأكد من أن المجلد موجود
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // المسار الكامل للصورة
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // نسخ الملف إلى المجلد بشكل غير متزامن
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }

                // حفظ المسار في الموديل
                donation.Image = "img/donationImages/" + uniqueFileName;
            }

            // تعيين UserId من الجلسة
            donation.UserId = SessionHelper.GetCurrentUserId(HttpContext);

            // التحقق من صحة البيانات
            if (ModelState.IsValid)
            {
                // إضافة الكائن إلى قاعدة البيانات بشكل غير متزامن
                await _context.Donations.AddAsync(donation);
                await _context.SaveChangesAsync();
            }
            // إعادة التوجيه إلى Donations مع تمرير categoryId
            return RedirectToAction("Donations", "Home", new { categoryId = donation.CategoryId });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveDonation(int id)
        {
            // العثور على التبرع في قاعدة البيانات
            var donation = await _context.Donations.FindAsync(id);
            if (donation != null)
            {
                // تكوين المسار الكامل للصورة
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img/donationImages", Path.GetFileName(donation.Image));

                // طباعة المسار للتأكد من صحته (يمكنك رؤية هذه الرسائل في السجلات)
                _logger.LogInformation($"Attempting to delete image at path: {imagePath}");

                // التحقق إذا كان الملف موجودًا ثم حذفه
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    _logger.LogInformation("Image deleted successfully.");
                }
                else
                {
                    _logger.LogWarning("Image not found.");
                }

                // حذف التبرع من قاعدة البيانات
                _context.Donations.Remove(donation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult service()
        {
            var categories = _context.Categorys.ToList();
            return View(categories);
        }
       
        public IActionResult team()
        {
            return View();
        }
        [HttpGet]
        public IActionResult VerifyEmail(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            user.IsEmailVerified = true; // تأكد من وجود حقل IsEmailVerified في جدول المستخدم
            _context.SaveChanges();

            TempData["Message"] = "Email verified successfully! You can now log in.";
            return RedirectToAction("Login");
        }
        public IActionResult Register()
        {
            return View();
        }
        // POST: Customer/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user, [FromServices] EmailService emailService)
        {
            if (ModelState.IsValid)
            {
                // التحقق من وجود البريد الإلكتروني، اسم المستخدم، أو رقم الهاتف في قاعدة البيانات
                if (_context.Users.Any(c => c.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                }

                if (_context.Users.Any(c => c.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Username is already in use.");
                }

                if (_context.Users.Any(c => c.Phone == user.Phone))
                {
                    ModelState.AddModelError("Phone", "Phone is already in use.");
                }

                // إذا لم تكن هناك أخطاء تحقق، يتم تنفيذ عملية التسجيل
                if (ModelState.ErrorCount == 0)
                {
                    // حفظ المستخدم في قاعدة البيانات
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // إرسال إيميل التحقق
                    var verificationLink = Url.Action("VerifyEmail", "Home",
                        new { userId = user.Id }, Request.Scheme);

                    var emailMessage = $@"
                <h1>Welcome, {user.Username}!</h1>
                <p>Thank you for registering. Please verify your email by clicking the link below:</p>
                <a href='{verificationLink}'>Verify Email</a>
            ";

                    // استدعاء خدمة إرسال الإيميل
                    await emailService.SendEmailAsync(user.Email, "Email Verification", emailMessage);

                    // ضبط قيم الـ Session بعد التسجيل الناجح
                    SessionHelper.SetAsUser(HttpContext);
                    SessionHelper.UserInfo(HttpContext, user);
 

                    // رسالة تأكيد التسجيل
                    TempData["Message"] = $"Registration successful! Please check your email to verify your account.";

                    // إعادة التوجيه إلى الصفحة الرئيسية
                    return RedirectToAction("Login", "Home");
                }
            }

            // إذا كان هناك أخطاء، إعادة عرض النموذج مع الأخطاء
            return View(user);
        }




        // GET: Customer/Login
        [HttpGet]

        public IActionResult Login()
        {
            return View();
        }

        // POST: Customer/Login
        [HttpPost]
        public IActionResult Login(string UsernameOEmail, string password)
        {
            // التحقق من وجود المستخدم في قاعدة البيانات
            var user = _context.Users.FirstOrDefault((c =>( c.Username == UsernameOEmail && c.Password == password)||( c.Email== UsernameOEmail && c.Password == password)));

            if (user != null)
            {
                if (!user.IsEmailVerified)
                {
                    ViewBag.Error = "Please verify your email before logging in.";

                    return View();
                }
                // ضبط قيم الـ Session
                HttpContext.Session.SetString("username", "John Doe");

                SessionHelper.SetAsUser(HttpContext);
                SessionHelper.UserInfo(HttpContext, user);



                // استخدام Session أو TempData لإظهار رسالة نجاح
                TempData["Message"] = $"Welcome, {user.Username}!";

                // إعادة التوجيه إلى Dashboard
                return RedirectToAction("Donate", "Home");
            }

            ViewBag.Error = "Invalid email/ or password.";
            return View();
        }



        // GET: Customer/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            // مسح جميع بيانات الـ Session
            SessionHelper.ClearSession(HttpContext);
            // استخدام TempData لإظهار رسالة تسجيل الخروج
            TempData["Message"] = "You have been logged out successfully.";

            // إعادة التوجيه إلى صفحة تسجيل الدخول
            return RedirectToAction("Index", "Home");
        }







        [HttpPost]
        public IActionResult UpdateRequestStatus(string status , int id)
        {
            
            var request = _context.Requests
                .Include(d=>d.Donation)
                .Where(r => r.Id == id).FirstOrDefault();
            
            if (request != null)
            {
                if(status == "Reject")
                {

                    request.Donation.DonationStatus = "Available";
                    _context.Requests.Remove(request);
                    _context.SaveChanges();
                }
                else if(status == "RejectedAndDelete")
                {
                    var donation = request.Donation;
                    if (donation != null)
                    {
                    _context.Requests.Remove(request);
                        //   _context.Donations.Remove(donation);
                        RemoveDonation(request.DonationId);
                    _context.SaveChanges();
                    }
                }
                else if(status== "Approved")
                    request.RequestStatus = status;
                _context.SaveChanges();

            }
            return RedirectToAction("Requests", "Home");
        }

        [HttpPost]
        public IActionResult RequestDonation(int id) {
            Donation donation =  _context.Donations.FirstOrDefault(d=>d.Id == id);
            int currentuserId =  SessionHelper.GetCurrentUserId(HttpContext) ;
            if (donation != null && donation.DonationStatus!= "Requested")
            {

                  
                Models.Request request = new Models.Request()
                {
                    
                    DonationId = donation.Id,
                    RequestDate = DateTime.Now,
                    RequestStatus = "Pending",
                    UserId = currentuserId
                };
                donation.DonationStatus = "Requested";
                _context.Requests.Add(request);
                _context.SaveChanges();
                }
            return RedirectToAction("Donate", "Home");
        }
    }
}