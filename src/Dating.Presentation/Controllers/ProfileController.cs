using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEFORmal.ua.Dating.ApplicationCore.Dtos;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;

namespace NEFORmal.ua.Dating.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ожидаем, что пользователь будет аутентифицирован
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IProfileService profileService, ILogger<ProfileController> logger)
        {
            _profileService = profileService;
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
            } catch
            {
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
        public async Task<IActionResult> CreateProfile([FromForm] CreateProfileDto profileForCreate)
        {
            try
            {
                await _profileService.CreateProfile(profileForCreate);
                return CreatedAtAction(nameof(GetProfileById), new { profileId = profileForCreate.Sid }, profileForCreate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // Обновление профиля
        [HttpPut("{profileId}")]
        public async Task<IActionResult> UpdateProfile(int profileId, [FromBody] UpdateProfileDto profile, CancellationToken cancellationToken)
        {
            try
            {
                await _profileService.UpdateProfile(profileId, profile, cancellationToken);
                return NoContent(); // Возвращаем успешный ответ без тела
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
