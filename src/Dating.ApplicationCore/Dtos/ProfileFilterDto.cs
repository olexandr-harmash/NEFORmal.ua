namespace NEFORmal.ua.Dating.ApplicationCore.Dtos;

public record ProfileFilterDto(string Sex, int AgeFrom, int AgeTo, int Page, int Limit);