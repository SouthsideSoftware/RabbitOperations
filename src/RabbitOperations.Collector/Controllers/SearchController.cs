using Microsoft.AspNet.Mvc;

namespace RabbitOperations.Collector.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}