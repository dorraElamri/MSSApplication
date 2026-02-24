using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/instances")]
    public class InstancesController : ControllerBase
    {
        private readonly IInstanceService _instanceService;
        private readonly IUserInstanceService _userInstanceService;

        public InstancesController(
            IInstanceService instanceService,
            IUserInstanceService userInstanceService)
        {
            _instanceService = instanceService;
            _userInstanceService = userInstanceService;
        }

        // ──────────────────────────────────────────────────────────────
        // Helper methods
        // ──────────────────────────────────────────────────────────────
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("Utilisateur non identifié dans le token");
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        // ──────────────────────────────────────────────────────────────
        //  GET /api/instances/my-instances
        //  → Liste des instances auxquelles l'utilisateur connecté a accès
        // ──────────────────────────────────────────────────────────────
        [HttpGet("my-instances")]
        public async Task<IActionResult> GetMyInstances()
        {
            var userId = GetCurrentUserId();
            var response = await _userInstanceService.GetInstancesOfUserAsync(userId);
            return Ok(response);
        }

        // ──────────────────────────────────────────────────────────────
        //  GET /api/instances
        //  → Liste TOUTES les instances (Admin only)
        // ──────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAllInstances()
        {
            if (!IsAdmin())
            {
                return Forbid("Accès réservé aux administrateurs");
            }

            var response = await _instanceService.GetAllInstancesAsync();
            return Ok(response);
        }

        // ──────────────────────────────────────────────────────────────
        //  GET /api/instances/{id}
        //  → Détail d'une instance (si accès ou Admin)
        // ──────────────────────────────────────────────────────────────
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetInstance(Guid id)
        {
            var userId = GetCurrentUserId();

            var accessResponse = await _userInstanceService.HasAccessAsync(userId, id);
            bool hasAccess = accessResponse.Success && accessResponse.Data;

            if (!IsAdmin() && !hasAccess)
            {
                return Forbid("Vous n'avez pas accès à cette instance");
            }

            var response = await _instanceService.GetInstanceByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }



        // ──────────────────────────────────────────────────────────────
        //  GET /api/instances/apikey/{apiKey}
        //  → Recherche par clé API (souvent utilisé par les agents de collecte)
        // ──────────────────────────────────────────────────────────────
        [HttpGet("apikey/{apiKey}")]
        [AllowAnonymous]   // ← souvent besoin pour les collecteurs de logs (à sécuriser autrement si besoin)
        public async Task<IActionResult> GetInstanceByApiKey(string apiKey)
        {
            var response = await _instanceService.GetInstanceByApiKeyAsync(apiKey);

            return Ok(response);
        }

        // ──────────────────────────────────────────────────────────────
        //  POST /api/instances
        //  → Création d'une instance (Admin only)
        // ──────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> CreateInstance([FromBody] CreateInstanceDto dto)
        {
            if (!IsAdmin())
            {
                return Forbid("Seuls les administrateurs peuvent créer une instance");
            }

            var response = await _instanceService.CreateInstanceAsync(dto);
            if (response.Success)
            {
                return CreatedAtAction(nameof(GetInstance), new { id = response.Data }, response);
            }
            return BadRequest(response);
        }

        // ──────────────────────────────────────────────────────────────
        //  PUT /api/instances/{id}
        //  → Mise à jour (Admin only)
        // ──────────────────────────────────────────────────────────────
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateInstance(Guid id, [FromBody] UpdateInstanceDto dto)
        {
            if (!IsAdmin())
            {
                return Forbid("Seuls les administrateurs peuvent modifier une instance");
            }

            var response = await _instanceService.UpdateInstanceAsync(id, dto);
            return Ok(response);
        }

        // ──────────────────────────────────────────────────────────────
        //  DELETE /api/instances/{id}
        //  → Suppression (Admin only)
        // ──────────────────────────────────────────────────────────────
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteInstance(Guid id)
        {
            if (!IsAdmin())
            {
                return Forbid("Seuls les administrateurs peuvent supprimer une instance");
            }

            var response = await _instanceService.DeleteInstanceAsync(id);
            return Ok(response);
        }

        // ──────────────────────────────────────────────────────────────
        //  POST /api/instances/{id}/users
        //  → Assigner un utilisateur à l'instance (Admin only)
        // ──────────────────────────────────────────────────────────────
        [HttpPost("{id:guid}/users")]
        public async Task<IActionResult> AssignUserToInstance(Guid id, [FromBody] AssignUserRequest request)
        {
            if (!IsAdmin())
            {
                return Forbid("Seuls les administrateurs peuvent assigner des utilisateurs");
            }

            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest(ApiResponse<bool>.Failure("Champ UserId obligatoire"));
            }

            var response = await _userInstanceService.AssignUserAsync(request.UserId, id);
            return Ok(response);
        }

        // ──────────────────────────────────────────────────────────────
        //  DELETE /api/instances/{id}/users/{userId}
        //  → Retirer un utilisateur de l'instance (Admin only)
        // ──────────────────────────────────────────────────────────────
        [HttpDelete("{id:guid}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromInstance(Guid id, string userId)
        {
            if (!IsAdmin())
            {
                return Forbid("Seuls les administrateurs peuvent retirer des utilisateurs");
            }

            var response = await _userInstanceService.RemoveUserAsync(userId, id);
            return Ok(response);
        }

        // ──────────────────────────────────────────────────────────────
        //  GET /api/instances/{id}/users
        //  → Liste des utilisateurs ayant accès à cette instance (Admin only)
        // ──────────────────────────────────────────────────────────────
        [HttpGet("{id:guid}/users")]
        public async Task<IActionResult> GetUsersOfInstance(Guid id)
        {
            if (!IsAdmin())
            {
                return Forbid("Seuls les administrateurs peuvent voir la liste des utilisateurs d'une instance");
            }

            var response = await _userInstanceService.GetUsersOfInstanceAsync(id);
            return Ok(response);
        }

        // ──────────────────────────────────────────────────────────────
        //  GET /api/instances/{id}/user-count
        //  → Nombre d'utilisateurs assignés (Admin only)
        // ──────────────────────────────────────────────────────────────
        [HttpGet("{id:guid}/user-count")]
        public async Task<IActionResult> GetInstanceUserCount(Guid id)
        {
            if (!IsAdmin())
            {
                return Forbid("Accès réservé aux administrateurs");
            }

            var response = await _userInstanceService.GetInstanceUserCountAsync(id);
            return Ok(response);
        }

        // ──────────────────────────────────────────────────────────────
        //  GET /api/instances/check-access/{id}
        //  → Vérifie si l'utilisateur courant a accès à cette instance
        // ──────────────────────────────────────────────────────────────
        [HttpGet("check-access/{id:guid}")]
        public async Task<IActionResult> CheckAccess(Guid id)
        {
            var userId = GetCurrentUserId();
            var response = await _userInstanceService.HasAccessAsync(userId, id);
            return Ok(response);
        }

        [HttpPost("{id}/regenerate-apikey")]
        public async Task<IActionResult> RegenerateApiKey(Guid id)
        {
            if (!IsAdmin())
            {
                return Forbid("Accès réservé aux administrateurs");
            }
            var result = await _instanceService.RegenerateApiKeyAsync(id);

            if (!result.Success)
            {
                // Instance non trouvée ou erreur
                if (result.Message.Contains("non trouvée"))
                    return NotFound(result);

                return StatusCode(500, result);
            }

            // ⚠️ On retourne la nouvelle clé UNE SEULE FOIS
            return Ok(result);
        }


    }


    // DTO pour assignation
    public class AssignUserRequest
    {
        public string UserId { get; set; } = string.Empty;
    }
}