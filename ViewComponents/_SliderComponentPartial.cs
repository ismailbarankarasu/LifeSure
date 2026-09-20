using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents
{
    public class _SliderComponentPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
