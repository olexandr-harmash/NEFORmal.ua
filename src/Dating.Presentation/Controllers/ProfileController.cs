using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using NEFORmal.ua.Dating.Presentation.Requests;

namespace NEFORmal.ua.Dating.Api.Controllers
{
    /// <summary>
    /// Controller responsible for handling user profile operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensures the user is authenticated
    public class ProfileController : ControllerBase
    {
        private readonly IUpdateProfileSagaUseCase _updateProfileSagaUseCase;
        private readonly ICreateProfileSagaUseCase _createProfileSagaService;
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfileController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileController"/> class.
        /// </summary>
        /// <param name="createProfileSagaService">Service for handling profile creation.</param>
        /// <param name="updateProfileSagaUseCase">Service for handling profile updates.</param>
        /// <param name="profileService">Service for handling profile data retrieval and deletion.</param>
        /// <param name="logger">Logger for logging actions within the controller.</param>
        public ProfileController(ICreateProfileSagaUseCase createProfileSagaService, IUpdateProfileSagaUseCase updateProfileSagaUseCase, IProfileService profileService, ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _createProfileSagaService = createProfileSagaService;
            _updateProfileSagaUseCase = updateProfileSagaUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Gets the current profile of the authenticated user based on their Sid from the JWT token.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor the cancellation of the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing the profile data or an error message.</returns>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var sid = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrEmpty(sid))
            {
                return Unauthorized("Sid not found in the token.");
            }

            try
            {
                var profile = await _profileService.GetProfileBySid(sid, cancellationToken);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile.");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Retrieves a profile by its unique identifier.
        /// </summary>
        /// <param name="profileId">The unique identifier of the profile to retrieve.</param>
        /// <param name="cancellationToken">Token to monitor the cancellation of the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing the profile data or a "not found" message.</returns>
        [HttpGet("{profileId}")]
        public async Task<IActionResult> GetProfileById(int profileId, CancellationToken cancellationToken)
        {
            var profile = await _profileService.GetProfileById(profileId, cancellationToken);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        /// <summary>
        /// Retrieves a list of profiles filtered by the provided criteria.
        /// </summary>
        /// <param name="filter">The filtering criteria for retrieving profiles.</param>
        /// <param name="cancellationToken">Token to monitor the cancellation of the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing a list of profiles matching the filter.</returns>
        [HttpGet]
        public async Task<IActionResult> GetProfilesByFilter([FromQuery] ProfileFilterDto filter, CancellationToken cancellationToken)
        {
            var profiles = await _profileService.GetProfileByFilter(filter, cancellationToken);
            return Ok(profiles);
        }

        /// <summary>
        /// Creates a new profile for the authenticated user.
        /// </summary>
        /// <param name="createProfileRequest">The data needed to create a new profile.</param>
        /// <param name="cancellationToken">Token to monitor the cancellation of the operation.</param>
        /// <returns>An <see cref="IActionResult"/> indicating whether the profile was successfully created.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromForm] CreateProfileRequest createProfileRequest, CancellationToken cancellationToken)
        {
            var sid = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrEmpty(sid))
            {
                return Unauthorized("Sid not found in the token.");
            }

            try
            {
                var profileForCreate = new CreateProfileDto(
                    sid,
                    createProfileRequest.Name,
                    createProfileRequest.Sex,
                    createProfileRequest.Bio,
                    createProfileRequest.Age
                );

                var formFiles = createProfileRequest.ProfilePhotos.ToList();

                var isOK = await _createProfileSagaService.ProcessProfileAsync(profileForCreate, formFiles, cancellationToken);

                if (isOK)
                {
                    return CreatedAtAction(nameof(GetProfileById), new { profileId = profileForCreate.Sid }, profileForCreate);
                }
                else
                {
                    return StatusCode(500, "Internal server error.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Updates an existing profile for the authenticated user.
        /// </summary>
        /// <param name="updateProfileRequest">The updated profile data.</param>
        /// <param name="cancellationToken">Token to monitor the cancellation of the operation.</param>
        /// <returns>An <see cref="IActionResult"/> indicating whether the profile was successfully updated.</returns>
        [HttpPut("{profileId}")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest updateProfileRequest, CancellationToken cancellationToken)
        {
            var sid = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrEmpty(sid))
            {
                return Unauthorized("Sid not found in the token.");
            }

            try
            {
                var profileForUpdate = new UpdateProfileDto(
                    updateProfileRequest.Name,
                    updateProfileRequest.Sex,
                    updateProfileRequest.Bio,
                    updateProfileRequest.Age
                );

                var formFiles = updateProfileRequest.ProfilePhotos.ToList();

                var isOk = await _updateProfileSagaUseCase.UpdateProfileAsync(sid, profileForUpdate, formFiles, cancellationToken);

                if (isOk)
                {
                    return NoContent(); // No content returned on success
                }
                else
                {
                    return StatusCode(500, "Internal server error.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Deletes the current profile of the authenticated user.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor the cancellation of the operation.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the profile deletion.</returns>
        [HttpDelete("{profileId}")]
        public async Task<IActionResult> DeleteProfile(CancellationToken cancellationToken)
        {
            var sid = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrEmpty(sid))
            {
                return Unauthorized("Sid not found in the token.");
            }

            try
            {
                await _profileService.DeleteProfile(sid, cancellationToken);
                return NoContent(); // No content returned on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
