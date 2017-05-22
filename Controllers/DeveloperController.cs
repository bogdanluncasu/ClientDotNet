using ClientExpandableObjectRecogniser.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClientExpandableObjectRecogniser.Controllers
{
    public class DeveloperController : Controller
    {
        HttpClient client;

        public DeveloperController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(Settings.URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public string GetToken()
        {
            var identity = (ClaimsIdentity)User.Identity;
            return identity.Claims.AsEnumerable().Where(c => c.Type.Equals("Token")).FirstOrDefault().Value;

        }


        public async Task<IActionResult> Index()
        {
            var token = GetToken();


            ICollection<KeyModel> keys = new List<KeyModel>();
            if (token != null)
            {
                TokenModel tm = new TokenModel();
                tm.token = token;
                var content = new StringContent(JsonConvert.SerializeObject(tm), Encoding.UTF8, "application/json");
                using (var responseMessage =
                       await client.PostAsync(Settings.URL + "developer/keys", content))
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    Dictionary<string, ICollection<KeyModel>> values =
                        JsonConvert.DeserializeObject<Dictionary<string, ICollection<KeyModel>>>(responseData);
                    if(values.ContainsKey("data"))
                        keys = values["data"];
                }
            }
            return View(keys);
        }


        [ActionName("Generate")]
        public async Task<ActionResult> Generate()
        {
            var token = GetToken();
            if (token != null)
            {
                TokenModel model = new TokenModel();
                model.token = token;

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                using (var responseMessage =
                       await client.PostAsync(Settings.URL + "developer/keys/generate", content))
                {
                }
            }
            return Redirect("/Developer");
        }

        [ActionName("Delete")]
        public async Task<ActionResult> Delete_Object(int id)
        {
            var token = GetToken();

            if (token != null)
            {
                TokenModel tm = new TokenModel();
                tm.token = token;
                tm.object_id = id;
                var content = new StringContent(JsonConvert.SerializeObject(tm), Encoding.UTF8, "application/json");
                using (var responseMessage =
                       await client.PostAsync(Settings.URL + "developer/keys/delete", content))
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                }
            }
            return Redirect("/Developer");
        }
    }
}
