using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents
{
    public class _TeamComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
