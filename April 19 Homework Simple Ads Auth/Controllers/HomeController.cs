using April_19_Homework_Simple_Ads_Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple_Ads_Auth.Data;
using System.Diagnostics;

namespace April_19_Homework_Simple_Ads_Auth.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAds;Integrated Security=true;";

        public IActionResult Index()
        {
            var repo = new UserRepository(_connectionString);
            var vm = new HomePageViewModel();
            vm.Ads = repo.GetAllAds();
            if (User.Identity.IsAuthenticated)
            {
                var currentUserEmail = User.Identity.Name;
                vm.CurrentUser = repo.GetByEmail(currentUserEmail);  
            }

            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewAd(SimpleAd ad)
        {
            var repo = new UserRepository(_connectionString);
            var currentUserEmail = User.Identity.Name;
            var user = repo.GetByEmail(currentUserEmail);

            repo.AddNewItem(ad, user.Id);

            return Redirect("Index");
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            var repo = new UserRepository(_connectionString);
            var currentUserEmail = User.Identity.Name;
            var user = repo.GetByEmail(currentUserEmail);

            var ads = repo.GetAdsByUser(user.Id);
            return View(ads);

        }

        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            var repo = new UserRepository(_connectionString);
            repo.DeleteAd(id);
            return Redirect("/Home/MyAccount");
        }
    }
}