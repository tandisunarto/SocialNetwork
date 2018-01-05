using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using SocialNetwork.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Globalization;

namespace SocialNetwork.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Shouts()
        {
            //// NEVER DO THIS
            //var username = HttpContext.Request.Cookies["username"]?.ToString();
            //// NEVER DO THIS
            //var password = HttpContext.Request.Cookies["password"]?.ToString();

            //if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            //{
            //    return RedirectToAction("login");
            //}
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //var shoutsResponse = await (await client.GetAsync($"http://localhost:1746/api/shouts?username={username}&password={password}")).Content.ReadAsStringAsync();
                var shoutsResponse = await (await client.GetAsync($"http://localhost:1746/api/shouts")).Content.ReadAsStringAsync();

                var shouts = JsonConvert.DeserializeObject<Shout[]>(shoutsResponse);
                
                return View(shouts);
            }
        }

        [Authorize]
        public async Task<IActionResult> TestAPI()
        {
            await RefreshTokens();
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");
            //httpClient.SetBearerToken(accessToken);
            // or
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

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

        private async Task RefreshTokens()
        {
            var authorizationServerInformation = await DiscoveryClient.GetAsync("http://localhost:1749");
            var client = new TokenClient(authorizationServerInformation.TokenEndpoint,
                "socialnetwork_code", "secret.code");
            var refreshToken = await HttpContext.Authentication.GetTokenAsync("refresh_token");
            var tokenResponse = await client.RequestRefreshTokenAsync(refreshToken);
            var identityToken = await HttpContext.Authentication.GetTokenAsync("id_token");
            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);

            var tokens = new[] {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = identityToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResponse.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenResponse.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                }
            };

            var authenticationInfo = await HttpContext.Authentication.GetAuthenticateInfoAsync("cookies");
            authenticationInfo.Properties.StoreTokens(tokens);
            await HttpContext.Authentication.SignInAsync("cookies",
                authenticationInfo.Principal, authenticationInfo.Properties);
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
