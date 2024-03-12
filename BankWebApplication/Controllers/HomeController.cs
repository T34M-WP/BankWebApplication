using BankWebApplication.Data;
using BankWebApplication.Models;
using BankWebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace BankWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public HomeController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public IActionResult Index()
        {
            // Decode JWT
            var token = HttpContext.Request.Cookies["logintoken"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Pass decoded JWT claims to the view
            var userId = jsonToken.Claims.First(claim => claim.Type == "nameid").Value;

            if (int.TryParse(userId, out int userIdInt))
            {
                var loggedInUser = _context.Users.FirstOrDefault(u => u.Id == userIdInt);
                if (loggedInUser != null)
                {
                    // Retrieve user's transactions where either the user is the sender or the receiver
                    var userTransactions = _context.Transactions
                        .Where(t => t.UserId == userIdInt || t.To == loggedInUser.Username)
                        .ToList();

                    // Assign user's transactions to ViewBag
                    ViewBag.UserTransactions = userTransactions;

                    // Assign logged in user to ViewBag
                    ViewBag.User = loggedInUser;
                }
                else
                {
                    // User not found
                }
            }

            return View();
        }
    }
}
