using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HW_4_23_SimpleAdsWLogin.web.Models;
using Microsoft.AspNetCore.Authorization;
using HW_4_23_SimpleAdsWLogin.data;

namespace HW_4_23_SimpleAdsWLogin.web.Controllers;

public class HomeController : Controller
{
    private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAdsWithLogin;Integrated Security=true;Trust Server Certificate=true;";

    public IActionResult Index()
    {
        var adsRepo = new AdsRepository(_connectionString);
        var vm = new IndexViewModel();
        vm.Ads = adsRepo.GetAllAds();
        return View(vm);
    }

    [Authorize]
    public IActionResult NewAdd()
    {
        return View();
    }

    [HttpPost]
    public IActionResult NewAd(Ad ad)
    {
        var userRepo = new UserRepository(_connectionString);
        var adsRepo = new AdsRepository(_connectionString);
        var user = userRepo.GetByEmail(User.Identity.Name);
        ad.UserId = user.Id;
        adsRepo.AddAd(ad);
        return Redirect("/");
    }

    public IActionResult DeleteAd(int id)
    {
        var adsRepo = new AdsRepository(_connectionString);
        adsRepo.DeleteAd(id);
        return Redirect("/");
    }
}
