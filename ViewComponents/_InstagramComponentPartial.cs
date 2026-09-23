using LifeSure.Services.Instagram;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents;

public sealed class _InstagramComponentPartial(IInstagramService instagramService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var posts = await instagramService.GetLatestAsync(HttpContext.RequestAborted);
        return View(posts);
    }
}