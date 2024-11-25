using System.Linq.Expressions;

using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Models;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace NEFORmal.ua.Dating.Infrastructure.Repository;

public class ProfileRepository : BaseRepository<DatingDbContext, Profile>, IProfileRepository
{
    private new readonly DatingDbContext _context;

    public ProfileRepository(DatingDbContext context) : base(context)
    {
        _context = context;
    }

    // Создание профиля
    public void CreateProfile(Profile profile)
    {
        Insert(profile);
    }

    // Удаление профиля по id
    public async Task DeleteProfile(int profileId, CancellationToken cancellationToken)
    {
        var profile = await base.GetByIdAsync(profileId, cancellationToken);
        if (profile != null)
        {
            Delete(profile);  // Удаляем профиль из DbSet
        }
    }

    public void DeleteProfile(Profile profile)
    {
        Delete(profile);  // Удаляем профиль из DbSet
    }

    // Получение профилей с фильтром и пагинацией
    public async Task<IEnumerable<Profile>> GetProfileByFilter(ProfileFilterDto filter, CancellationToken cancellationToken)
    {
        // Создание фильтра в виде Expression
        Expression<Func<Profile, bool>> filterExpression = x =>
            x.Age >= filter.AgeFrom &&
            x.Age <= filter.AgeTo &&
            x.Sex == filter.Sex;

        // Получение данных с фильтрацией и пагинацией
        var profiles = await Get(
            filter: filterExpression,
            skip: filter.Page * filter.Limit,  // Правильный расчет skip
            take: filter.Limit,
            cancellationToken: cancellationToken
        );

        // Возврат результата с пагинацией
        return profiles;
    }


    // Получение профиля по ID
    public ValueTask<Profile?> GetProfileById(int profileId, CancellationToken cancellationToken)
    {
        return GetByIdAsync(profileId, cancellationToken);
    }

    public async Task<Profile?> GetProfileBySid(string sid, CancellationToken cancellationToken)
    {
        return await _context.Profiles.FirstOrDefaultAsync(p => p.Sid == sid);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    // Обновление профиля
    public void UpdateProfile(Profile profile, CancellationToken cancellationToken)
    {
        Update(profile);
    }
}
