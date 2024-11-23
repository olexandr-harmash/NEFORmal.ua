namespace NEFORmal.ua.Dating.ApplicationCore.Dtos;

public record ProfileFilterDto(string Sex, int AgeFrom, int AgeTo, int Page, int Limit)
{
    public string Sex { get; init; } = Sex;
    public int AgeFrom { get; init; } = AgeFrom;
    public int AgeTo { get; init; } = AgeTo;
    public int Page { get; init; } = Page;
    public int Limit { get; init; } = Limit;
}
