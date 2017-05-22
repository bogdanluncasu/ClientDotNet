using ClientExpandableObjectRecogniser.Models;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Text;
using ClientExpandableObjectRecogniser;

namespace ScrumPlatform.Controllers
{
    public class UserAccountController : Controller
    {
        HttpClient client;

        public UserAccountController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(Settings.URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RegisterAccount(UserModel user)
        {
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(Settings.URL + "users", user);

            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseData);
                return View((object)values["message"]);
                
            }
            return View((object)"Something went wrong");
        }

        public ActionResult Login()
        { 
            return View();
        }

        [ActionName("Login")]
        [HttpPost]
        public async Task<ActionResult> Login_Post(UserModel model)
        {
            HttpResponseMessage responseMessage;
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            using (responseMessage =
                   await client.PostAsync(Settings.URL + "auth", content)) { 


                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    Dictionary<string, Dictionary<string,string>> values = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(responseData);

                    if (values.ContainsKey("message"))
                    {
                        ViewBag.Error = values["message"];
                        return View(new UserModel());
                    }
                    else
                    {
                        Dictionary<string, string> user = values["user"];
                        var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, model.username),
                        new Claim("Token", user["token"])
                            }, "ApplicationCookie");

                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.Authentication.SignInAsync("Cookies", principal,
                            new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                                IsPersistent = false,
                                AllowRefresh = false
                            });

                        return Redirect("/");
                    }

                }
            }

            ViewBag.Error = "Something went wrong";
            return View(new UserModel());
            
           

        }

        public ActionResult LogOff()
        {
            HttpContext.Authentication.SignOutAsync("Cookies");
            return Redirect("/");
        }
    }
}