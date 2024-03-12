using BankWebApplication.Data;
using BankWebApplication.Models;
using BankWebApplication.Services;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AccountController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    // หน้า Register
    public IActionResult Register()
    {
        return View();
    }

    // การลงทะเบียน (POST)
    [HttpPost]
    public IActionResult Register(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        return View(user);
    }

    // หน้า Login
    public IActionResult Login()
    {
        // ตรวจสอบว่ามีคุกกี้ logintoken หรือไม่
        if (HttpContext.Request.Cookies.ContainsKey("logintoken"))
        {
            // หากมี ให้ลบคุกกี้ออก
            HttpContext.Response.Cookies.Delete("logintoken");
            // Redirect ไปยังหน้า Login
            return RedirectToAction("Login");
        }
        return View();
    }

    // ล็อกอิน (POST)
    [HttpPost]
    public IActionResult Login(User user)
    {
        var loggedInUser = _context.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
        if (loggedInUser != null)
        {
            // สร้าง token
            var token = _jwtService.GenerateToken(loggedInUser.Id);
            // เก็บ token ในคุกกี้
            HttpContext.Response.Cookies.Append("logintoken", token);

            // Redirect ไปยังหน้า Index ของ Controller Decode
            return RedirectToAction("Index", "Home");
        }
        // ถ้าไม่พบผู้ใช้หรือรหัสผ่านไม่ถูกต้อง ให้แสดงข้อความผิดพลาด
        ModelState.AddModelError(string.Empty, "Invalid username or password");
        return View(user);
    }

    // ออกจากระบบ
    public IActionResult Logout()
    {
        // ตรวจสอบว่ามีคุกกี้ logintoken หรือไม่
        if (HttpContext.Request.Cookies.ContainsKey("logintoken"))
        {
            // หากมี ให้ลบคุกกี้ออก
            HttpContext.Response.Cookies.Delete("logintoken");
            // Redirect ไปยังหน้า Login
            return RedirectToAction("Login");
        }
        // ถ้าไม่มีคุกกี้ logintoken ให้เปลี่ยนเส้นทางไปยังหน้า Login
        return RedirectToAction("Login");
    }
}