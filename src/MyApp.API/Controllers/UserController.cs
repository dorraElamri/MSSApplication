using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Application.Services.User;

namespace MyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Toutes les routes nécessitent authentification par défaut
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IAuthService _authService;

        public UserController(UserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        // --------------------
        // LOGIN
        // --------------------
        [HttpPost("login")]
        [AllowAnonymous] // accessible sans token
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
        //[Authorize(Policy = "AdminOnly")] // Seul l'admin peut créer un utilisateur
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
        [Authorize(Policy = "AdminOnly")] // Seul l'admin peut voir tous les utilisateurs
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsers();
            return Ok(ApiResponse<IEnumerable<ApplicationUser>>.SuccessResponse(users, "Users retrieved successfully"));
        }

        // --------------------
        // GET USER BY ID
        // --------------------
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")] // Seul l'admin peut accéder à n'importe quel utilisateur
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(ApiResponse<ApplicationUser>.SuccessResponse(user!, "User retrieved successfully"));
        }

        // --------------------
        // UPDATE USER
        // --------------------
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")] // Seul l'admin peut mettre à jour un utilisateur
        public async Task<IActionResult> Update(string id, UpdateUserDto dto)
        {
            var user = await _userService.UpdateUser(id, dto);
            return Ok(ApiResponse<ApplicationUser>.SuccessResponse(user, "User updated successfully"));
        }

        // --------------------
        // DELETE USER
        // --------------------
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")] // Seul l'admin peut supprimer un utilisateur
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteUser(id);
            return Ok(ApiResponse<string>.SuccessResponse(null!, "User deleted successfully"));
        }
    }
}
