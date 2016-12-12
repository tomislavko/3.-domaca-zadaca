using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zadatak2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Zadatak2.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;


        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

/*
        [Authorize]
        public async Task<IActionResult> TopSecret()
        {
            // this is one way to check if client is authorized.
            // it will be always true because [Authorize] won't let 
            // anonymous users to log in.
            // bool alwaysTrue = HttpContext.user.Indentity.IsAuthenticated;

            // fetch current user information.
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user);
        }
        */


        public IActionResult Index()
        {

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
