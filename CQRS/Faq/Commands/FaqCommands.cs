using LifeSure.CQRS.Faqs.Models;

namespace LifeSure.CQRS.Faqs.Commands;

public record SaveFaqCommand(int? Id, FaqInput Input);

public record DeleteFaqCommand(int Id);