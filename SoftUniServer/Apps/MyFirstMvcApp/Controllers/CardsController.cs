using MyFirstMvcApp.ViewModels;
using SUS.HTTP;
using SUS.MvcFramework;

namespace MyFirstMvcApp.Controllers
{
    public class CardsController : Controller
    {
        public HttpResponse Add()
        {
            return View();
        }

        [HttpPost("/Cards/Add")]
        public HttpResponse DoAdd()
        {
            var request = Request;
            var viewModel = new DoAddViewModel
            {
                Attack = int.Parse(Request.FormData["attack"]),
                Health = int.Parse(Request.FormData["health"])
            };

            return View(viewModel);
        }

        public HttpResponse All()
        {
            return View();
        }

        public HttpResponse Collection()
        {
            return View();
        }
    }
}
