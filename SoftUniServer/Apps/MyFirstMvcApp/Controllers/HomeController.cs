using BattleCards.ViewModels;
using SUS.HTTP;
using SUS.MvcFramework;
using System;
using System.Linq;
using System.Text;

namespace BattleCards.Controllers
{ 
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public HttpResponse Index()
        {
            var viewModel = new IndexViewModel();
            viewModel.CurrentYear = DateTime.UtcNow.Year;
            viewModel.Message = "Welcome to Battle Cards";
            if (Request.Session.ContainsKey("about"))
            {
                viewModel.Message += " (You have been on about page.)";
            }
            return View(viewModel);
        }

        public HttpResponse About()
        {
            Request.Session["about"] = "yes";
            return View();
        }
    }
}
