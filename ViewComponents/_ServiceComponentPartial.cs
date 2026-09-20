using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents
{
    public class _ServiceComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
