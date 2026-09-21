using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public abstract class AdminControllerBase : Controller
{
}