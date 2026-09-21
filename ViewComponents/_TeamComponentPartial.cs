using LifeSure.Mediator.TeamMembers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LifeSure.ViewComponents;

public class _TeamComponentPartial(ISender sender)
    : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var members = await sender.Send(
            new GetTeamMembersQuery(OnlyActive: true),
            HttpContext.RequestAborted);

        return View(members);
    }
}