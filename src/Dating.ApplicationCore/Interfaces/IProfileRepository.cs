using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Models;

namespace NEFORmal.ua.Dating.ApplicationCore.Interfaces;

public interface IProfileRepository
{
    // Создание нового профиля
    void CreateProfile(Profile profile);

    // Удаление профиля по ID
    Task DeleteProfile(int profileId, CancellationToken cancellationToken);

    // Получение профиля по ID
    ValueTask<Profile?> GetProfileById(int profileId, CancellationToken cancellationToken);

    Task<Profile?> GetProfileBySid(string sid, CancellationToken cancellationToken);

    // Получение профилей с фильтром и пагинацией
    Task<IEnumerable<Profile>> GetProfileByFilter(ProfileFilterDto filter, CancellationToken cancellationToken);

    // Обновление профиля
    void UpdateProfile(Profile profile, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    void DeleteProfile(Profile profile);
}
