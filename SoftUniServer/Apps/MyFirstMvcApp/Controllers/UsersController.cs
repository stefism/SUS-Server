using SUS.HTTP;
using SUS.MvcFramework;
using System;

namespace MyFirstMvcApp.Controllers
{
    public class UsersController : Controller
    {
        public HttpResponse Login()
        {
            return View();
        }

        public HttpResponse Register()
        {
            return View();
        }

        [HttpPost]
        public HttpResponse DoLogin()
        {
            return Redirect("/");
        }
    }
}
