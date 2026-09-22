using LifeSure.Mediator.ContactMessages.Results;
using MediatR;

namespace LifeSure.Mediator.ContactMessages.Queries;

public record GetContactMessagesQuery(
    int Page = 1) : IRequest<ContactMessageListResult>;

public record GetContactMessageByIdQuery(
    int Id) : IRequest<ContactMessageDetailResult?>;