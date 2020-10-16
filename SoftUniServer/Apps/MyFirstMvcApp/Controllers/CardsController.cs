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
        private ApplicationDbContext db;

        public CardsController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public HttpResponse Add()
        {
            if (!IsUserSignedIn())
            {
                return Redirect("/Users/Login");
            }

            return View();
        }

        [HttpPost("/Cards/Add")]
        public HttpResponse DoAdd(AddCardInputModel model)
        {
            if (!IsUserSignedIn())
            {
                return Redirect("/Users/Login");
            }

            //var dbContext = new ApplicationDbContext(); The dependency inversion principle is not observed! All we need, must be filled in the constructor!

            if (Request.FormData["name"].Length < 5)
            {
                return Error("Name should be at least 5 character long.");
            }

            db.Cards.Add(new Card
            {
                Attack = model.Attack,
                Health = model.Health,
                Description = model.Description,
                Name = model.Name,
                ImageUrl = model.Image,
                Keyword = model.Keyword,
            });

            db.SaveChanges();

            return Redirect("/Cards/All");
        }

        public HttpResponse All()
        {
            if (!IsUserSignedIn())
            {
                return Redirect("/Users/Login");
            }

            //var db = new ApplicationDbContext(); The dependency inversion principle is not observed! All we need, must be filled in the constructor!

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
            if (!IsUserSignedIn())
            {
                return Redirect("/Users/Login");
            }

            return View();
        }
    }
}
