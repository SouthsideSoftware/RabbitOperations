using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using RabbitOperations.Collector.Models;

namespace RabbitOperations.Collector.Controllers
{
    public class HomeController : Controller
    { 
        private static List<CommentModel> comments;

        static HomeController()
        {
            comments = new List<CommentModel>
            {
                new CommentModel {Author = "Tom", Text = "This is the first comment -- "},
                new CommentModel {Author = "Phil", Text = "This is *another* comment"}
            };
        }
   
        public IActionResult Index()
        {
            return View();
        }

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

        public IActionResult React()
        {
            return View(comments);
        }

        public IActionResult Superman()
        {
            return View(comments);
        }

        [ResponseCache(NoStore = true, Duration = 0)]
        public ActionResult Comments()
        {
            comments[0].Text += ".";
            return Json(comments);
        }

        [HttpPost]
        public ActionResult AddComment(CommentModel comment)
        {
            comments.Add(comment);
            return Content("Success :)");
        }
    }
}