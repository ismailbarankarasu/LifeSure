using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents
{
    public class _AboutComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
