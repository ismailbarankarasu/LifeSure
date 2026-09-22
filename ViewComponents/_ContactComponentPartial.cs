using LifeSure.Mediator.ContactMessages.Models;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents;

public class _ContactComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(new ContactMessageInput());
    }
}