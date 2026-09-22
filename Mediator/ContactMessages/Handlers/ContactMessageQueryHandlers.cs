using LifeSure.Data;
using LifeSure.Mediator.ContactMessages.Queries;
using LifeSure.Mediator.ContactMessages.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.ContactMessages.Handlers;

public class GetContactMessagesQueryHandler(LifeSureDbContext context)
    : IRequestHandler<GetContactMessagesQuery, ContactMessageListResult>
{
    public async Task<ContactMessageListResult> Handle(
        GetContactMessagesQuery request,
        CancellationToken cancellationToken)
    {
        const int pageSize = 20;

        var query = context.ContactMessages.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var totalPages = Math.Max(
            1,
            (int)Math.Ceiling(totalCount / (double)pageSize));

        var page = Math.Clamp(request.Page, 1, totalPages);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ContactMessageListItem(
                x.Id,
                x.FullName,
                x.Subject,
                x.Status,
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new ContactMessageListResult(
            items,
            page,
            totalPages,
            totalCount);
    }
}

public class GetContactMessageByIdQueryHandler(LifeSureDbContext context)
    : IRequestHandler<GetContactMessageByIdQuery, ContactMessageDetailResult?>
{
    public Task<ContactMessageDetailResult?> Handle(
        GetContactMessageByIdQuery request,
        CancellationToken cancellationToken)
    {
        return context.ContactMessages
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new ContactMessageDetailResult(
                x.Id,
                x.FullName,
                x.Email,
                x.PhoneNumber,
                x.Subject,
                x.Message,
                x.Status,
                x.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }
}