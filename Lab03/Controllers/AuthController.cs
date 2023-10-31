using Lab03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics; 

namespace Lab03.Controllers
{
    public  abstract  class AuthController : Controller
    {
          String loginUserId = null;
          String loginName = null; 
  

        public String LoginName
        {
            get
            {
                return loginName;
            }
        }

        public String LoginUserId
        {
            get
            {
                return loginUserId;
            }
        }


        public AuthController()
        {


        }

        //override the default login page
        public string GetModelStateValidMessage()
        {
            string msg = "";

            if (!ModelState.IsValid)
            {

                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Console.WriteLine($"Model Error: {error.ErrorMessage}");
                    }
                }


                var errors = ModelState.Values.SelectMany(v => v.Errors);
                msg = errors.Aggregate("", (current, error) => current+ "\n" + error.ErrorMessage);

            }

            return msg;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Path.ToString().StartsWith("/Users/Login")
                || context.HttpContext.Request.Path.ToString().StartsWith("/Users/Create")
                )
            {
                return;
            }

            //get current user's login name
            loginName = HttpContext.Session.GetString("loginName");
            loginUserId = HttpContext.Session.GetString("loginUserId");


            //if user is not logged in, redirect to login page
            if (loginName == null)
            { 
                context.Result = new RedirectResult("/Users/Login"); 
            }
             
            //save login name and id to viewbag
            ViewBag.LoginName = loginName;
            ViewBag.LoginUserId = loginUserId;



             










            base.OnActionExecuting(context);
        }



        public IActionResult ShowError(String msg)
        {
            ViewBag.Message = msg;
            return View( "../Auth/Msg");
        }

        public IActionResult ShowModelStateValidMessage( )
        {
            var msg= this.GetModelStateValidMessage();
            ViewBag.Message =  msg;
            return View("../Auth/Msg");
        }

    }
}