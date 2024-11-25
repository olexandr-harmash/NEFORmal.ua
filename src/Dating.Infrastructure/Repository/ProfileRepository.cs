using System.Linq.Expressions;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Models;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace NEFORmal.ua.Dating.Infrastructure.Repository
{
    /// <summary>
    /// Repository for managing profile data in the database.
    /// </summary>
    public class ProfileRepository : BaseRepository<DatingDbContext, Profile>, IProfileRepository
    {
        private new readonly DatingDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for interacting with the profiles.</param>
        public ProfileRepository(DatingDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new profile in the database.
        /// </summary>
        /// <param name="profile">The profile to be created.</param>
        public void CreateProfile(Profile profile)
        {
            Insert(profile);
        }

        /// <summary>
        /// Deletes a profile by its unique identifier from the database.
        /// </summary>
        /// <param name="profileId">The identifier of the profile to be deleted.</param>
        /// <param name="cancellationToken">The cancellation token for the async operation.</param>
        public async Task DeleteProfile(int profileId, CancellationToken cancellationToken)
        {
            var profile = await base.GetByIdAsync(profileId, cancellationToken);
            if (profile != null)
            {
                Delete(profile);  // Deletes the profile from the DbSet
            }
        }

        /// <summary>
        /// Deletes the provided profile from the database.
        /// </summary>
        /// <param name="profile">The profile to be deleted.</param>
        public void DeleteProfile(Profile profile)
        {
            Delete(profile);  // Deletes the profile from the DbSet
        }

        /// <summary>
        /// Retrieves a list of profiles based on the provided filter and applies pagination.
        /// </summary>
        /// <param name="filter">The filter criteria for fetching profiles.</param>
        /// <param name="cancellationToken">The cancellation token for the async operation.</param>
        /// <returns>A collection of profiles matching the filter criteria and pagination.</returns>
        public async Task<IEnumerable<Profile>> GetProfileByFilter(ProfileFilterDto filter, CancellationToken cancellationToken)
        {
            // Create a filter expression based on the provided filter criteria
            Expression<Func<Profile, bool>> filterExpression = x =>
                x.Age >= filter.AgeFrom &&
                x.Age <= filter.AgeTo &&
                x.Sex == filter.Sex;

            // Retrieve filtered profiles with pagination
            var profiles = await Get(
                filter: filterExpression,
                skip: filter.Page * filter.Limit,  // Correct calculation for skip
                take: filter.Limit,
                cancellationToken: cancellationToken
            );

            // Return the filtered and paginated profiles
            return profiles;
        }

        /// <summary>
        /// Retrieves a profile by its unique identifier.
        /// </summary>
        /// <param name="profileId">The identifier of the profile to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token for the async operation.</param>
        /// <returns>A task representing the asynchronous operation, with the profile if found; otherwise, null.</returns>
        public ValueTask<Profile?> GetProfileById(int profileId, CancellationToken cancellationToken)
        {
            return GetByIdAsync(profileId, cancellationToken);
        }

        /// <summary>
        /// Retrieves a profile by its session identifier (SID).
        /// </summary>
        /// <param name="sid">The session identifier of the profile to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token for the async operation.</param>
        /// <returns>The profile if found; otherwise, null.</returns>
        public async Task<Profile?> GetProfileBySid(string sid, CancellationToken cancellationToken)
        {
            return await _context.Profiles.FirstOrDefaultAsync(p => p.Sid == sid);
        }

        /// <summary>
        /// Saves any changes made to the context in the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for the async operation.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing profile in the database.
        /// </summary>
        /// <param name="profile">The profile to be updated.</param>
        /// <param name="cancellationToken">The cancellation token for the async operation.</param>
        public void UpdateProfile(Profile profile, CancellationToken cancellationToken)
        {
            Update(profile);
        }
    }
}
