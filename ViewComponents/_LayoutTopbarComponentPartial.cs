using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents
{
    public class _LayoutTopbarComponentPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
