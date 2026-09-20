using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents
{
    public class _FeatureComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
