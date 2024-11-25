using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;

namespace NEFORmal.ua.Dating.ApplicationCore.Interfaces;

public interface IUpdateProfileSagaUseCase
{
    Task<bool> UpdateProfileAsync(string sid, UpdateProfileDto profileForUpdate, List<IFormFile> formFiles, CancellationToken cancellationToken);
    void Compensate();
}