using FreePosts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FreePost.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace FreePosts.Web.Controllers
{

    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=FreeBay;Integrated Security=true;";

        public IActionResult Index()
        {
            
            var db = new FreebayDb(_connectionString);
            List<int> list = HttpContext.Session.Get<List<int>>("Ids");
            if (list == null)
            {
                list = new List<int>();
            }
            HttpContext.Session.Set("Ids", list);
            var vm = new GetAllViewModel
            {
                Ids = list,
                Posts = db.GetAllPosts()
            };

            return View(vm);
        }

        public IActionResult NewAd()
        {

            return View();
        }
        [HttpPost]
        public IActionResult NewAd(Post post)
        {
            FreebayDb db = new FreebayDb(_connectionString);
            if (post.Name != null) {db.AddPost(post, true); }
            else {db.AddPost(post, false); }


            var ids = HttpContext.Session.Get<List<int>>("Ids");
            if (ids == null)
            {
                ids = new List<int>();
            }

            ids.Add(post.Id);
            HttpContext.Session.Set("Ids", ids);
            return Redirect("/Home/Index");
        }

        public IActionResult Delete(int id)
        {
            var db = new FreebayDb(_connectionString);
            db.Delete(id);
            return Redirect("/Home/Index");

        }
    }

        public static class SessionExtensions
        {
            public static void Set<T>(this ISession session, string key, T value)
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }

            public static T Get<T>(this ISession session, string key)
            {
                string value = session.GetString(key);
                return value == null ? default(T) :
                    JsonConvert.DeserializeObject<T>(value);
            }
        }


    }

