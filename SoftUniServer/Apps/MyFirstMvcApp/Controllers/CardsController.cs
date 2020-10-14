using BattleCards.Data;
using BattleCards.ViewModels;
using Newtonsoft.Json.Serialization;
using SUS.HTTP;
using SUS.MvcFramework;
using System.Linq;

namespace BattleCards.Controllers
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
            var dbContext = new ApplicationDbContext();

            if (Request.FormData["name"].Length < 5)
            {
                return Error("Name should be at least 5 character long.");
            }

            dbContext.Cards.Add(new Card
            {
                Attack = int.Parse(Request.FormData["attack"]),
                Health = int.Parse(Request.FormData["health"]),
                Description = Request.FormData["description"],
                Name = Request.FormData["name"],
                ImageUrl = Request.FormData["image"],
                Keyword = Request.FormData["keyword"],
            });

            dbContext.SaveChanges();

            return Redirect("/");
        }

        public HttpResponse All()
        {
            var db = new ApplicationDbContext();
            var cardsViewModel = db.Cards
                .Select(db => new CardViewModel
                {
                    Name = db.Name,
                    Attack = db.Attack,
                    Health = db.Health,
                    ImageUrl = db.ImageUrl,
                    Type = db.Keyword,
                    Description = db.Description
                }).ToList();

            return View(cardsViewModel);
        }

        public HttpResponse Collection()
        {
            return View();
        }
    }
}
