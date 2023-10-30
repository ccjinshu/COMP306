using Lab03.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lab03.Controllers
{
    public class HomeController : AuthController
    {


        public IActionResult Index()
        {
            ////get current user's login name
            //var loginName = HttpContext.Session.GetString("loginName");

            ////if user is not logged in, redirect to login page
            //if (loginName != null && loginName != "")
            //{ 
            //    ViewBag.Message = "Login successfully!";
            //    ViewBag.LoginName = loginName;
            //}
            return View();


        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}