using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;

namespace NEFORmal.ua.Dating.ApplicationCore.Interfaces;

public interface ICreateProfileSagaUseCase
{
    Task<bool> ProcessProfileAsync(CreateProfileDto profileForCreate, List<IFormFile> formFiles, CancellationToken cancellationToken);
    void Compensate();
}