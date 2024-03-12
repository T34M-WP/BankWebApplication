using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using BankWebApplication.Data;
using BankWebApplication.Models;
using System.IdentityModel.Tokens.Jwt;

namespace BankWebApplication.Controllers
{
    public class TransactionController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionController(AppDbContext context)
        {
            _context = context;
        }

        /*
        // GET: Transactions/Index
        public IActionResult Index()
        {
            var token = HttpContext.Request.Cookies["logintoken"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userIdClaim = jsonToken.Claims.First(claim => claim.Type == "nameid").Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                var userTransactions = _context.Transactions.Where(t => t.UserId == userId).ToList(); // กรองและดึงรายการ Transaction ของผู้ใช้ที่เข้าสู่ระบบ
                ViewBag.UserTransactions = userTransactions; // นำรายการ Transaction ไปเก็บใน ViewBag
            }
            else
            {
                ViewBag.UserTransactions = new List<Transaction>(); // ถ้าไม่สามารถระบุผู้ใช้ได้ สร้างรายการ Transaction ว่างๆ และเก็บใน ViewBag
            }
            return View("~/Views/Transaction/Index.cshtml");
        }*/


        // GET: Transactions/Deposit
        public IActionResult Deposit()
        {
            return View();
        }

        // POST: Transactions/Deposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deposit(decimal amount)
        {
            // Decode JWT
            var token = HttpContext.Request.Cookies["logintoken"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Pass decoded JWT claims to the view
            var userIdClaim = jsonToken.Claims.First(claim => claim.Type == "nameid").Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    // บันทึกข้อมูลการฝากลงใน Transaction.Model
                    _context.Transactions.Add(new Transaction
                    {
                        DateTime = DateTime.Now,
                        UserId = user.Id,
                        Action = "Deposit",
                        From = "Self",
                        To = "Null",
                        Amount = amount
                    });

                    // เพิ่มยอดเงินในบัญชีผู้ใช้
                    user.Balance += amount;

                    // บันทึกการเปลี่ยนแปลงลงในฐานข้อมูล
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(Index), "Home"); // Redirect to home page
        }

        // GET: Transactions/Withdraw
        public IActionResult Withdraw()
        {
            return View();
        }

        // POST: Transactions/Withdraw
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Withdraw(decimal amount)
        {
            // Decode JWT
            var token = HttpContext.Request.Cookies["logintoken"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Pass decoded JWT claims to the view
            var userIdClaim = jsonToken.Claims.First(claim => claim.Type == "nameid").Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    // ตรวจสอบว่ายอดเงินเพียงพอหรือไม่
                    if (user.Balance >= amount)
                    {
                        // บันทึกข้อมูลการถอนเงินลงใน Transaction.Model
                        _context.Transactions.Add(new Transaction
                        {
                            DateTime = DateTime.Now,
                            UserId = user.Id,
                            Action = "Withdraw",
                            From = "Self",
                            To = "Self",
                            Amount = amount
                        });

                        // ลดยอดเงินในบัญชีผู้ใช้
                        user.Balance -= amount;

                        // บันทึกการเปลี่ยนแปลงลงในฐานข้อมูล
                        _context.SaveChanges();

                        return RedirectToAction(nameof(Index), "Home"); // Redirect to home page
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Insufficient balance.");
                    }
                }
            }

            return View();
        }

        // GET: Transactions/Transfer
        public IActionResult Transfer()
        {
            return View();
        }

        // POST: Transactions/Transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Transfer(int recipientId, decimal amount)
        {
            // Decode JWT
            var token = HttpContext.Request.Cookies["logintoken"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Pass decoded JWT claims to the view
            var userIdClaim = jsonToken.Claims.First(claim => claim.Type == "nameid").Value;
            if (int.TryParse(userIdClaim, out int senderId))
            {
                var sender = _context.Users.FirstOrDefault(u => u.Id == senderId);
                var recipient = _context.Users.FirstOrDefault(u => u.Id == recipientId);

                if (sender != null && recipient != null)
                {
                    // ตรวจสอบว่ายอดเงินเพียงพอหรือไม่
                    if (sender.Balance >= amount)
                    {
                        // บันทึกข้อมูลการโอนเงินลงใน Transaction.Model ของผู้โอน
                        _context.Transactions.Add(new Transaction
                        {
                            DateTime = DateTime.Now,
                            UserId = sender.Id,
                            Action = "Transfer",
                            From = sender.Username,
                            To = recipient.Username,
                            Amount = amount
                        });

                        // ลดยอดเงินในบัญชีผู้โอน
                        sender.Balance -= amount;

                        // เพิ่มยอดเงินในบัญชีผู้รับ
                        recipient.Balance += amount;

                        // บันทึกการเปลี่ยนแปลงลงในฐานข้อมูล
                        _context.SaveChanges();

                        return RedirectToAction(nameof(Index), "Home"); // Redirect to home page
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Insufficient balance.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Sender or recipient not found.");
                }
            }

            return View();
        }



    }
}