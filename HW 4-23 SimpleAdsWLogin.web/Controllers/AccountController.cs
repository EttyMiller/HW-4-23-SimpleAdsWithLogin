using HW_4_23_SimpleAdsWLogin.data;
using HW_4_23_SimpleAdsWLogin.web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HW_4_23_SimpleAdsWLogin.web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAdsWithLogin;Integrated Security=true;Trust Server Certificate=true;";

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var repo = new UserRepository(_connectionString);
            repo.AddUser(user, password);
            return RedirectToAction("index", "home");
        }

        public IActionResult Login()
        {
            var vm = new LoginViewModel();
            if (TempData["message"] != null)
            {
                vm.Message = (string)TempData["message"];
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new UserRepository(_connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid login";
                return Redirect("/account/login");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", ClaimTypes.Email, "roles"))
                ).Wait();

            return Redirect("/account/myaccount");

        }

        [Authorize]
        public IActionResult MyAccount()
        {
            var userRepo = new UserRepository(_connectionString);
            var user = userRepo.GetByEmail(User.Identity.Name);

            var adsRepo = new AdsRepository(_connectionString);
            var vm = new MyAccountViewModel();
            vm.Ads = adsRepo.GetUsersAds(user);
            vm.Name = $"{user.FirstName} {user.LastName}";

            return View(vm);
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
    }
}
