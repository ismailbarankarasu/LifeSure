using LifeSure.CQRS.Sliders.Models;

namespace LifeSure.CQRS.Sliders.Commands;

public record UpdateSliderCommand(int Id, SliderInput Input);