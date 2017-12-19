using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using SocialNetwork.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace SocialNetwork.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Shouts()
        {
            // NEVER DO THIS
            var username = HttpContext.Request.Cookies["username"]?.ToString();
            // NEVER DO THIS
            var password = HttpContext.Request.Cookies["password"]?.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return RedirectToAction("login");
            }

            using (var client = new HttpClient())
            {
                var shoutsResponse = await (await client.GetAsync($"http://localhost:33917/api/shouts?username={username}&password={password}")).Content.ReadAsStringAsync();

                var shouts = JsonConvert.DeserializeObject<Shout[]>(shoutsResponse);
                
                return View(shouts);
            }
        }

        public async Task<IActionResult> TestAPI()
        {
            var httpClient = new HttpClient();

            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");
            httpClient.SetBearerToken(accessToken);
            // or
            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var testTask = await httpClient.GetAsync("http://localhost:1746/test");
            var testReadStringTask = testTask.Content.ReadAsStringAsync();
            TestAPI model = JsonConvert.DeserializeObject<TestAPI>(testReadStringTask.Result);
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // NEVER DO THIS
            // NEVER DO THIS
            // NEVER DO THIS
            // NEVER DO THIS
            HttpContext.Response.Cookies.Append("username", username);

            // NEVER DO THIS
            // NEVER DO THIS
            // NEVER DO THIS
            HttpContext.Response.Cookies.Append("password", password);

            return RedirectToAction("Shouts");
        }

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Home/Index"
            }, "oidc");
        }

        public IActionResult Logout()
        {
            //await HttpContext.Authentication.SignOutAsync("oidc ");
            //await HttpContext.Authentication.SignOutAsync("cookies");
            //return Redirect("~/");
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "/Home/Index"
            }, "cookies", "oidc");
        }

        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
