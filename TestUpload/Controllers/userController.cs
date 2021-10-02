﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;
using TestUpload.Service;


namespace TestUpload.Controllers
{
    public class userController : Controller
    {
        private readonly IuserService _iuserService ;
        private readonly ILoginService _loginService;
        public userController(IuserService iuserService,ILoginService loginService)
        {
            _iuserService = iuserService;
            _loginService = loginService;
        }

        [HttpGet("/user")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("user/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("user/login")]
        public IActionResult Logined()
        {
            string username = Request.Form["login"].ToString();
            string password = Request.Form["pass"].ToString();
            User principal = _loginService.GetLogin(username, password);
            if(principal != null)
            {
                return Ok(principal);
            }
            return Ok(500);
        }
        [HttpGet("user/register")]
        public IActionResult register()
        {
            return View();
        }
        [HttpPost("user/register")]
        public IActionResult Registerd()
        {
            User principal = new User();
            string password, retype;
            password = HttpContext.Request.Form["pass"].ToString();
            retype = HttpContext.Request.Form["ret"].ToString();
            if(password==retype)
            {
                try
                {
                    string fn, ln, Email, Rules, username;
                    DateTime brithday;
                    bool male;
                    fn = HttpContext.Request.Form["Fn"].ToString();
                    ln = HttpContext.Request.Form["Ln"].ToString();
                    Email = HttpContext.Request.Form["Email"].ToString();
                    Rules = "user";
                    username = HttpContext.Request.Form["username"].ToString();
                    male = bool.Parse(HttpContext.Request.Form["Gender"].ToString());
                    brithday = DateTime.Parse(HttpContext.Request.Form["BR"].ToString());
                    principal = new User
                    {
                        Rules = Rules,
                        BrithDay = brithday,
                        Email = Email,
                        Firstname = fn,
                        Lastname = ln,
                        Login = new Login
                        {
                            Password = password,
                            Username = username
                        },
                        Male = male
                    };
                    var t = _iuserService.Register(principal);
                    if (t)
                    {
                        ViewBag.result = "Registerd Successful";
                        return View();
                    }
                    ViewBag.result = "Registerd UnSuccessful";
                    return View();
                }
                catch(Exception x)
                {
                    Console.WriteLine(x.Message);
                    ViewBag.result = "Registerd Corruped";
                    return View();
                }

            }
            ViewBag.result = "Password Retype Incorrect";
            return View();
        }
    }
}
