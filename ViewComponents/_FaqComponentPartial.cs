using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents
{
    public class _FaqComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
