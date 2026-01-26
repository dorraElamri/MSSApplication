using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Application.Interfaces.IOtpService;
using MyApp.Application.Services.User;
using MyApp.Domain.Entities;

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

        //// --------------------
        //// RESET PASSWORD USING OTP
        //// --------------------
        //[HttpPost("reset-password")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        //{
        //    // Vérifie OTP
        //    var isOtpValid = await _otpService.VerifyOtpAsync(dto.Email, dto.OtpCode, OtpPurpose.ForgotPassword);
        //    if (!isOtpValid)
        //        return BadRequest(ApiResponse<string>.Fail("Invalid or expired OTP"));

        //    // Met à jour le mot de passe
        //    var updated = await _userService.UpdatePasswordByEmailAsync(dto.Email, dto.NewPassword);
        //    if (!updated)
        //        return NotFound(ApiResponse<string>.Fail("User not found"));

        //    return Ok(ApiResponse<string>.Success(null, "Password reset successfully"));
        //}


    }
}
