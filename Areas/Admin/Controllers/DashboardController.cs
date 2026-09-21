using Microsoft.AspNetCore.Mvc;

namespace LifeSure.Areas.Admin.Controllers;

public class DashboardController : AdminControllerBase
{
    public IActionResult Index()
    {
        return View();
    }
}