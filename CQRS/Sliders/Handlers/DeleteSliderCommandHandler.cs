using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Sliders.Commands;
using LifeSure.Data;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Sliders.Handlers;

public class DeleteSliderCommandHandler(LifeSureDbContext context)
    : ICommandHandler<DeleteSliderCommand, bool>
{
    public async Task<bool> HandleAsync(
        DeleteSliderCommand command,
        CancellationToken cancellationToken = default)
    {
        var slider = await context.Sliders
            .SingleOrDefaultAsync(
                x => x.Id == command.Id,
                cancellationToken);

        if (slider is null)
        {
            return false;
        }

        context.Sliders.Remove(slider);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}