using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using ClientExpandableObjectRecogniser.Models;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Text;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ClientExpandableObjectRecogniser.Controllers
{
    public class ObjectsController : Controller
    {
        HttpClient client;

        public ObjectsController()
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


            ICollection<ObjectModel> coll = new List<ObjectModel>();
            if (token != null)
            {
                TokenModel tm = new TokenModel();
                tm.token = token;
                var content = new StringContent(JsonConvert.SerializeObject(tm), Encoding.UTF8, "application/json");
                using (var responseMessage =
                       await client.PostAsync(Settings.URL + "image/objects", content))
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    Dictionary<string, ICollection<ObjectModel>> values = JsonConvert.DeserializeObject<Dictionary<string, ICollection<ObjectModel>>>(responseData);
                    coll = values["data"];
                }
            }
            return View(coll);
        }
        [ActionName("Add")]
        public ActionResult Add()
        {
            return View();
        }

        [ActionName("Add")]
        [HttpPost]
        public async Task<string> Add_Post([FromBody]FileModel model)
        {
            var token = GetToken();
            var message = "";
            if (token != null)
            {
                model.token = token;
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                using (var responseMessage =
                       await client.PostAsync(Settings.URL + "image/objects/add", content))
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    Dictionary<string, string> values = JsonConvert.DeserializeObject < Dictionary < string, string>> (responseData);
                    if (values.ContainsKey("message"))
                        message = values["message"];
                    else if (values.ContainsKey("details"))
                        message = values["details"];
                }
            } 
            return message;
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
                       await client.PostAsync(Settings.URL + "image/objects/delete", content))
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                }
            }
            return Redirect("/Objects");
        }
    }
}
