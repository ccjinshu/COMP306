﻿using Lab03.Models;
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



        public override void OnActionExecuting(ActionExecutingContext context)
        {

            //get current user's login name
            loginName = HttpContext.Session.GetString("loginName");
            loginUserId = HttpContext.Session.GetString("loginUserId");


            //if user is not logged in, redirect to login page
            if (loginName == null)
            {
                //if route is start with /Users, do not redirect to login page
                if (context.HttpContext.Request.Path.ToString().StartsWith("/Movies"))
                {
                    context.Result = new RedirectResult("/Users/Login");
                }

                

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

    }
}