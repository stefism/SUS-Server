﻿using SUS.HTTP;
using SUS.MvcFramework;
using System;

namespace MyFirstMvcApp.Controllers
{
    public class UsersController : Controller
    {
        public HttpResponse Login(HttpRequest request)
        {
            return View();
        }

        public HttpResponse Register(HttpRequest request)
        {
            return View();
        }

        [HttpPost]
        public HttpResponse DoLogin(HttpRequest request)
        {
            return Redirect("/");
        }
    }
}
