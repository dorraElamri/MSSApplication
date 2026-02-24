using Domain.Entities;
using Microsoft.Extensions.Logging;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.Application.Services
{
    public class UserInstanceService : IUserInstanceService
    {
        private readonly IUserInstanceRepository _repository;
        private readonly ILogger<UserInstanceService> _logger;

        public UserInstanceService(
            IUserInstanceRepository repository,
            ILogger<UserInstanceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> HasAccessAsync(string userId, Guid instanceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<bool>.Failure("Identifiant utilisateur requis");
                }

                bool hasAccess = await _repository.ExistsAsync(userId, instanceId);
                return ApiResponse<bool>.SuccessResponse(hasAccess, hasAccess ? "Accès autorisé" : "Accès non autorisé");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification d'accès pour {UserId} → {InstanceId}", userId, instanceId);
                return ApiResponse<bool>.Failure("Erreur lors de la vérification d'accès");
            }
        }

        public async Task<ApiResponse<bool>> AssignUserAsync(string userId, Guid instanceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<bool>.Failure("Identifiant utilisateur requis");
                }

                if (await _repository.ExistsAsync(userId, instanceId))
                {
                    _logger.LogInformation("Utilisateur {UserId} est déjà assigné à l'instance {InstanceId}", userId, instanceId);
                    return ApiResponse<bool>.SuccessResponse(true, "Utilisateur déjà assigné");
                }

                var link = new UserInstance
                {
                    UserId = userId,
                    InstanceId = instanceId
                };

                await _repository.AddAsync(link);

                _logger.LogInformation("Utilisateur {UserId} assigné à l'instance {InstanceId}", userId, instanceId);
                return ApiResponse<bool>.SuccessResponse(true, "Utilisateur assigné avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'assignation {UserId} → {InstanceId}", userId, instanceId);
                return ApiResponse<bool>.Failure("Erreur lors de l'assignation de l'utilisateur");
            }
        }


        //comment
        public async Task<ApiResponse<bool>> RemoveUserAsync(string userId, Guid instanceId)
        {
            try
            {
                var link = await _repository.GetLinkAsync(userId, instanceId);

                if (link == null)
                {
                    _logger.LogWarning("Tentative de suppression d'un lien inexistant : {UserId} → {InstanceId}", userId, instanceId);
                    return ApiResponse<bool>.SuccessResponse(true, "Aucun lien à supprimer (déjà absent)");
                }

                await _repository.DeleteAsync(link);

                _logger.LogInformation("Utilisateur {UserId} retiré de l'instance {InstanceId}", userId, instanceId);
                return ApiResponse<bool>.SuccessResponse(true, "Utilisateur retiré avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du retrait {UserId} → {InstanceId}", userId, instanceId);
                return ApiResponse<bool>.Failure("Erreur lors du retrait de l'utilisateur");
            }
        }

        public async Task<ApiResponse<IReadOnlyList<Instance>>> GetInstancesOfUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<IReadOnlyList<Instance>>.Failure("Identifiant utilisateur requis");
                }

                var instances = await _repository.GetInstancesForUserAsync(userId);

                if (instances.Count == 0)
                {
                    return ApiResponse<IReadOnlyList<Instance>>.Failure("Aucune instance associée à cet utilisateur");
                }

                return ApiResponse<IReadOnlyList<Instance>>.SuccessResponse(instances, $"{instances.Count} instance(s) trouvée(s)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des instances pour {UserId}", userId);
                return ApiResponse<IReadOnlyList<Instance>>.Failure("Erreur lors de la récupération des instances");
            }
        }

        public async Task<ApiResponse<IReadOnlyList<ApplicationUser>>> GetUsersOfInstanceAsync(Guid instanceId)
        {
            try
            {
                var users = await _repository.GetUsersForInstanceAsync(instanceId);

                if (users.Count == 0)
                {
                    return ApiResponse<IReadOnlyList<ApplicationUser>>.Failure("Aucun utilisateur associé à cette instance");
                }

                return ApiResponse<IReadOnlyList<ApplicationUser>>.SuccessResponse(users, $"{users.Count} utilisateur(s) trouvé(s)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs pour l'instance {InstanceId}", instanceId);
                return ApiResponse<IReadOnlyList<ApplicationUser>>.Failure("Erreur lors de la récupération des utilisateurs");
            }
        }

        public async Task<ApiResponse<bool>> IsUserLinkedToAnyInstanceAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<bool>.Failure("Identifiant utilisateur requis");
                }

                bool hasLink = await _repository.UserHasAnyLinkAsync(userId);
                return ApiResponse<bool>.SuccessResponse(hasLink, hasLink ? "L'utilisateur est lié à au moins une instance" : "Aucune instance liée");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur vérification liens pour {UserId}", userId);
                return ApiResponse<bool>.Failure("Erreur lors de la vérification des liens");
            }
        }

        public async Task<ApiResponse<int>> GetInstanceUserCountAsync(Guid instanceId)
        {
            try
            {
                int count = await _repository.CountUsersForInstanceAsync(instanceId);
                return ApiResponse<int>.SuccessResponse(count, $"Nombre d'utilisateurs : {count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur comptage utilisateurs pour {InstanceId}", instanceId);
                return ApiResponse<int>.Failure("Erreur lors du comptage des utilisateurs");
            }
        }
    }
}