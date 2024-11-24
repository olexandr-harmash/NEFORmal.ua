using Microsoft.AspNetCore.Http;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;

namespace NEFORmal.ua.Dating.ApplicationCore.Interfaces;

public interface IUpdateProfileSagaService
{
    Task<bool> UpdateProfileAsync(int profileId, UpdateProfileDto profileForUpdate, CancellationToken cancellationToken);
    void Compensate(List<string> formFiles, List<string> lastFiles);
}