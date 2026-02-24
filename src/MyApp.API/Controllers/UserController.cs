using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Application.Interfaces.IOtpService;
using MyApp.Application.Services.User;
//using MyApp.Domain.Entities;

namespace MyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Toutes les routes nécessitent authentification par défaut
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;


        public UserController(UserService userService, IAuthService authService, IOtpService otpService)
        {
            _userService = userService;
            _authService = authService;
            _otpService = otpService;
        }

        // --------------------
        // LOGIN
        // --------------------
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful"));
        }

        // --------------------
        // REFRESH TOKEN
        // --------------------
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token refreshed successfully"));
        }

        // --------------------
        // CREATE USER
        // --------------------
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            var user = await _userService.AddUser(dto);
            return Ok(ApiResponse<ApplicationUser>.SuccessResponse(user, "User created successfully"));
        }

        // --------------------
        // GET ALL USERS
        // --------------------
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsers();
            return Ok(ApiResponse<IEnumerable<ApplicationUser>>.SuccessResponse(users, "Users retrieved successfully"));
        }

        // --------------------
        // GET USER BY ID
        // --------------------
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(ApiResponse<ApplicationUser>.SuccessResponse(user!, "User retrieved successfully"));
        }

        // --------------------
        // UPDATE USER
        // --------------------
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(string id, UpdateUserDto dto)
        {
            var user = await _userService.UpdateUser(id, dto);
            return Ok(ApiResponse<ApplicationUser>.SuccessResponse(user, "User updated successfully"));
        }

        // --------------------
        // DELETE USER
        // --------------------
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteUser(id);
            return Ok(ApiResponse<string>.SuccessResponse(null!, "User deleted successfully"));
        }

        // --------------------
        // GET CURRENT USER (ME)
        // --------------------
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var user = await _userService.GetUserById(userId);
            if (user == null)
                return NotFound();

            return Ok(ApiResponse<ApplicationUser>.SuccessResponse(user, "Current user retrieved successfully"));
        }

        // --------------------
        // OTP: GENERATE & SEND (FORGOT PASSWORD OR EMAIL VERIFICATION)
        // --------------------
        [HttpPost("otp/generate")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateOtp([FromBody] OtpRequestDto dto)
        {
            Console.WriteLine($"Email: {dto.Email}, Purpose: {dto.Purpose}");
            await _otpService.GenerateAndSendOtpAsync(dto.Email, dto.Purpose);
            return Ok(ApiResponse<string>.SuccessResponse(null, "OTP sent successfully"));
        }

        // --------------------
        // OTP: VERIFY
        // --------------------
        [HttpPost("otp/verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDto dto)
        {
            var isValid = await _otpService.VerifyOtpAsync(dto.Email, dto.Code, dto.Purpose);
            if (!isValid)
                return BadRequest(ApiResponse<string>.Failure("Invalid or expired OTP"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "OTP verified successfully"));
        }



        [HttpPost("change-password")]
        [AllowAnonymous] // accessible via OTP
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                var user = await _userService.ChangePasswordWithOtpAsync(dto, _otpService);
                return Ok(ApiResponse<ApplicationUser>.SuccessResponse(user, "Mot de passe changé avec succès"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Failure(ex.Message));
            }
        }




        // GET: api/User/{id}/roles
        [HttpGet("{id}/roles")]
        [Authorize(Policy = "AdminOnly")]           // ← ou [Authorize] si tu veux que n'importe quel utilisateur connecté puisse voir les rôles d'un autre
        public async Task<IActionResult> GetUserRoles(string id)
        {
            try
            {
                var roles = await _userService.GetUserRolesAsync(id);
                return Ok(ApiResponse<IList<string>>.SuccessResponse(roles, "Rôles récupérés avec succès"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Failure(ex.Message));
            }
        }

        // GET: api/User/me/roles   ← très utile pour le profil de l'utilisateur connecté
        [HttpGet("me/roles")]
        [Authorize]     // Doit être connecté
        public async Task<IActionResult> GetCurrentUserRoles()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("ID utilisateur non trouvé dans le token");

            try
            {
                var roles = await _userService.GetUserRolesAsync(userId);
                return Ok(ApiResponse<IList<string>>.SuccessResponse(roles, "Rôles de l'utilisateur courant récupérés"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Failure(ex.Message));
            }
        }

    }
}
