using donate.data;
using donate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace donate.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Login
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            // Check if user exists
            if (email == "Admin@gmail.com" && password == "mo000000")
            {
                HttpContext.Session.SetString("isAdmin", "True");

                // Example of session or temp data usage
                TempData["Message"] = $"Welcome, {email}!";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminViewModel
            {
                Categories = await _context.Categorys.ToListAsync(),
                Users = await _context.Users.ToListAsync(),
                Donations = await _context.Donations.Include(p => p.Category).ToListAsync()
            };

            var isAdmin = HttpContext.Session.GetString("isAdmin");
            if (isAdmin == null)
                return NotFound();

            return View(viewModel);
        }

        // POST: Add Category
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                await _context.Categorys.AddAsync(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Add User
        [HttpPost]
        public async Task<IActionResult> AddUser(User User)
        {
            if (ModelState.IsValid)
            {
                await _context.Users.AddAsync(User);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Add Donation
        [HttpPost]
        public async Task<IActionResult> AddDonation(Donation Donation)
        {
            if (ModelState.IsValid)
            {
                await _context.Donations.AddAsync(Donation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Update Category
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categorys.Update(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Update User
        [HttpPost]
        public async Task<IActionResult> UpdateUser(User User)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(User);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Update Donation
        [HttpPost]
        public async Task<IActionResult> UpdateDonation(Donation Donation)
        {
            if (ModelState.IsValid)
            {
                _context.Donations.Update(Donation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Category
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categorys.FindAsync(id);
            if (category != null)
            {
                _context.Categorys.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete User
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var User = await _context.Users.FindAsync(id);
            if (User != null)
            {
                _context.Users.Remove(User);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Donation
        [HttpPost]
        public async Task<IActionResult> DeleteDonation(int id)
        {
            var Donation = await _context.Donations.FindAsync(id);
            if (Donation != null)
            {
                _context.Donations.Remove(Donation);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Category deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }

    public class AdminViewModel
    {
        public List<Category> Categories { get; set; }
        public List<User> Users { get; set; }
        public List<Donation> Donations { get; set; }
    }
}
