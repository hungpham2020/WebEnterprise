using Microsoft.AspNetCore.Mvc;

namespace WebEnterprise.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
