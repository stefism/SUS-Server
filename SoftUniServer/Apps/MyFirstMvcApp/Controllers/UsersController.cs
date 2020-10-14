using BattleCards.Services;
using SUS.HTTP;
using SUS.MvcFramework;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BattleCards.Controllers
{
    public class UsersController : Controller
    {
        private UserService userService;

        public UsersController()
        {
            userService = new UserService();
        }

        public HttpResponse Login()
        {
            return View();
        }

        [HttpPost("/Users/Login")]
        public HttpResponse DoLogin()
        {
            var username = Request.FormData["username"];
            var password = Request.FormData["password"];
            var userId = userService.GetUserId(username, password);
            
            if (userId == null)
            {
                return Error("Invalid username or password");
            }

            SignIn(userId);
            return Redirect("/");
        }

        public HttpResponse Register()
        {
            return View();
        }

        [HttpPost("/Users/Register")]
        public HttpResponse DoRegister()
        {
            var username = Request.FormData["username"];
            var email = Request.FormData["email"];
            var password = Request.FormData["password"];
            var confirmPassword = Request.FormData["confirmPassword"];

            if (string.IsNullOrEmpty(username) || username.Length < 5
                            || username.Length > 20)
            {
                return Error("Username must be between 5 and 20 characters.");
            }

            if (!Regex.IsMatch(username, @"[a-zA-Z0-9\.]+"))
            {
                return Error("Username contain invalid character.");
            }

            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            {
                return Error("Invalid email.");
            }

            if (password == null || password.Length < 6 || password.Length > 20)
            {
                return Error("Password must be between 6 and 20 characters long.");
            }

            if (password != confirmPassword)
            {
                return Error("Passwords should be the same.");
            }

            if (!userService.IsUsernameAvailable(username))
            {
                return Error("This username already exist.");
            }

            if (!userService.IsEmailAvailable(email))
            {
                return Error("This email already exist.");
            }

            var userId = userService.CreateUser(username, email, password);

            return Redirect("/Users/Login");
        }
        
        public HttpResponse Logout()
        {
            if (!IsUserSignedIn())
            {
                return Error("Only logged-in users can logout.");
            }
            
            SignOut();
            return Redirect("/");
        }   
    }
}
