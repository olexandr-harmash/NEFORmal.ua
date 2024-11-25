using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using NEFORmal.ua.Dating.Presentation.Requests;

namespace NEFORmal.ua.Dating.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ожидаем, что пользователь будет аутентифицирован
    public class ProfileController : ControllerBase
    {
        private readonly IUpdateProfileSagaUseCase _updateProfileSagaUseCase;
        private readonly ICreateProfileSagaUseCase _createProfileSagaService;
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ICreateProfileSagaUseCase createProfileSagaService, IUpdateProfileSagaUseCase updateProfileSagaUseCase, IProfileService profileService, ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _createProfileSagaService = createProfileSagaService;
            _updateProfileSagaUseCase = updateProfileSagaUseCase;
            _logger = logger;
        }

        // Получение текущего профиля с использованием Sid из JWT токена
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
            } catch (Exception ex)
            {
                  _logger.LogError(ex, "Error getting profile.");
                return StatusCode(500);
            }
        }

        // Получение профиля по ID
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

        // Получение профилей с фильтром
        [HttpGet]
        public async Task<IActionResult> GetProfilesByFilter([FromQuery] ProfileFilterDto filter, CancellationToken cancellationToken)
        {
            var profiles = await _profileService.GetProfileByFilter(filter, cancellationToken);
            return Ok(profiles);
        }

        // Создание профиля
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

        // Обновление профиля
        [HttpPut("{profileId}")]
        public async Task<IActionResult> UpdateProfile(int profileId, [FromForm] UpdateProfileRequest updateProfileRequest, CancellationToken cancellationToken)
        {
            var sid = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrEmpty(sid))
            {
                return Unauthorized("Sid not found in the token.");
            }

            try
            {
                var profileForUpdate = new UpdateProfileDto(
                    sid,
                    updateProfileRequest.Name,
                    updateProfileRequest.Sex,
                    updateProfileRequest.Bio,
                    updateProfileRequest.Age
                );

                var formFiles = updateProfileRequest.ProfilePhotos.ToList();

                var isOk = await _updateProfileSagaUseCase.UpdateProfileAsync(profileId, profileForUpdate, formFiles, cancellationToken);

                if (isOk)
                {
                    return NoContent(); // Возвращаем успешный ответ без тела
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

        // Удаление профиля
        [HttpDelete("{profileId}")]
        public async Task<IActionResult> DeleteProfile(int profileId, CancellationToken cancellationToken)
        {
            try
            {
                await _profileService.DeleteProfile(profileId, cancellationToken);
                return NoContent(); // Успешное удаление
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
