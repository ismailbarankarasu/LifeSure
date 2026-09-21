using LifeSure.CQRS.Abouts.Models;

namespace LifeSure.CQRS.Abouts.Commands;

public record SaveAboutCommand(int? Id, AboutInput Input);

public record DeleteAboutCommand(int Id);