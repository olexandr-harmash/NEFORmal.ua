using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;

namespace NEFORmal.ua.Dating.ApplicationCore.Interfaces;

public interface ICreateProfileSagaService
{
    Task<bool> ProcessProfileAsync(CreateProfileDto profileForCreate, CancellationToken cancellationToken);
    void Compensate(List<string> formFiles);
}