namespace NEFORmal.ua.Dating.ApplicationCore.Dtos;

public record PaginatedItemsDto<T>(IEnumerable<T> Items, bool IsEnd)
{
    public IEnumerable<T> Items { get; init; } = Items;

    public bool IsEnd { get; init; } = IsEnd;
}
