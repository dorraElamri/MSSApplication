using Microsoft.Extensions.Logging;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using System.Security.Cryptography;

namespace MyApp.Application.Services;

public class InstanceService : IInstanceService
{
    private readonly IInstanceRepository _instanceRepository;
    private readonly ILogger<InstanceService> _logger;

    public InstanceService(
        IInstanceRepository instanceRepository,
        ILogger<InstanceService> logger)
    {
        _instanceRepository = instanceRepository;
        _logger = logger;
    }

    // =========================
    // GET ALL
    // =========================
    public async Task<ApiResponse<List<Instance>>> GetAllInstancesAsync()
    {
        try
        {
            var instances = await _instanceRepository.GetAllAsync();

            if (!instances.Any())
                return ApiResponse<List<Instance>>.Failure("Aucune instance trouvée");

            return ApiResponse<List<Instance>>.SuccessResponse(
                instances.ToList(),
                $"{instances.Count()} instances récupérées"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur récupération instances");
            return ApiResponse<List<Instance>>.Failure("Erreur interne");
        }
    }

    // =========================
    // GET BY ID
    // =========================
    public async Task<ApiResponse<Instance>> GetInstanceByIdAsync(Guid id)
    {
        try
        {
            var instance = await _instanceRepository.GetByIdAsync(id);

            if (instance == null)
                return ApiResponse<Instance>.Failure($"Instance {id} non trouvée");

            return ApiResponse<Instance>.SuccessResponse(instance, "Instance récupérée");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur récupération instance {Id}", id);
            return ApiResponse<Instance>.Failure("Erreur interne");
        }
    }

    // =========================
    // GET BY API KEY
    // =========================
    public async Task<ApiResponse<Instance>> GetInstanceByApiKeyAsync(string apiKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return ApiResponse<Instance>.Failure("ApiKey requise");

            var instance = await _instanceRepository.GetByApiKeyAsync(apiKey);

            if (instance == null || !instance.IsActive)
                return ApiResponse<Instance>.Failure("Instance invalide ou inactive");

            // 🔥 Mise à jour last used
            instance.ApiKeyLastUsedAt = DateTime.UtcNow;
            await _instanceRepository.UpdateAsync(instance);

            return ApiResponse<Instance>.SuccessResponse(instance, "Instance trouvée");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur recherche ApiKey");
            return ApiResponse<Instance>.Failure("Erreur interne");
        }
    }

    // =========================
    // GET USER INSTANCES
    // =========================
    public async Task<ApiResponse<List<Instance>>> GetInstancesForUserAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ApiResponse<List<Instance>>.Failure("UserId requis");

            var instances = await _instanceRepository.GetInstancesForUserAsync(userId);

            if (!instances.Any())
                return ApiResponse<List<Instance>>.Failure("Aucune instance");

            return ApiResponse<List<Instance>>.SuccessResponse(
                instances.ToList(),
                $"{instances.Count()} instances récupérées"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur instances user {UserId}", userId);
            return ApiResponse<List<Instance>>.Failure("Erreur interne");
        }
    }

    // =========================
    // CREATE
    // =========================
    public async Task<ApiResponse<Guid>> CreateInstanceAsync(CreateInstanceDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Host))
            {
                return ApiResponse<Guid>.Failure("Host est obligatoire");
            }

            // 🔐 Génération ApiKey unique
            string apiKey;
            do
            {
                apiKey = Generate();
            }
            while (await _instanceRepository.GetByApiKeyAsync(apiKey) != null);

            var entity = new Instance
            {
                Id = Guid.NewGuid(),
                Host = dto.Host,
                ApplicationName = dto.ApplicationName,
                Description = dto.Description,
                LogPath = dto.LogPath,
                Environment = dto.Environment,

                ApiKey = apiKey,
                ApiKeyCreatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _instanceRepository.AddAsync(entity);

            _logger.LogInformation("Instance créée {Id} | Host: {Host}", entity.Id, entity.Host);

            return ApiResponse<Guid>.SuccessResponse(entity.Id, "Instance créée");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur création instance");
            return ApiResponse<Guid>.Failure("Erreur interne");
        }
    }

    // =========================
    // UPDATE
    // =========================
    public async Task<ApiResponse<bool>> UpdateInstanceAsync(Guid id, UpdateInstanceDto dto)
    {
        try
        {
            var instance = await _instanceRepository.GetByIdAsync(id);
            if (instance == null)
                return ApiResponse<bool>.Failure($"Instance {id} non trouvée");

            // 🔄 Mise à jour partielle
            if (!string.IsNullOrWhiteSpace(dto.ApplicationName))
                instance.ApplicationName = dto.ApplicationName;

            if (!string.IsNullOrWhiteSpace(dto.Host))
                instance.Host = dto.Host;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                instance.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.LogPath))
                instance.LogPath = dto.LogPath;

            if (dto.Environment.HasValue)
                instance.Environment = dto.Environment.Value;

            if (dto.IsActive.HasValue)
                instance.IsActive = dto.IsActive.Value;

            instance.UpdatedAt = DateTime.UtcNow;

            await _instanceRepository.UpdateAsync(instance);

            _logger.LogInformation("Instance mise à jour {Id}", id);

            return ApiResponse<bool>.SuccessResponse(true, "Instance mise à jour");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur update instance {Id}", id);
            return ApiResponse<bool>.Failure("Erreur interne");
        }
    }

    // =========================
    // DELETE
    // =========================
    public async Task<ApiResponse<bool>> DeleteInstanceAsync(Guid id)
    {
        try
        {
            var instance = await _instanceRepository.GetByIdAsync(id);
            if (instance == null)
                return ApiResponse<bool>.Failure($"Instance {id} non trouvée");

            await _instanceRepository.DeleteAsync(id);

            _logger.LogInformation("Instance supprimée {Id}", id);

            return ApiResponse<bool>.SuccessResponse(true, "Instance supprimée");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur suppression instance {Id}", id);
            return ApiResponse<bool>.Failure("Erreur interne");
        }
    }

    // =========================
    // API KEY GENERATOR
    // =========================
    public static string Generate(int length = 32)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        return Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");
    }

    // =========================
    // REGENERATE API KEY
    // =========================
    public async Task<ApiResponse<string>> RegenerateApiKeyAsync(Guid id)
    {
        try
        {
            var instance = await _instanceRepository.GetByIdAsync(id);
            if (instance == null)
                return ApiResponse<string>.Failure($"Instance {id} non trouvée");

            string newApiKey;
            do
            {
                newApiKey = Generate();
            }
            while (await _instanceRepository.GetByApiKeyAsync(newApiKey) != null);

            instance.ApiKey = newApiKey;
            instance.ApiKeyCreatedAt = DateTime.UtcNow;
            instance.ApiKeyLastUsedAt = null;
            instance.UpdatedAt = DateTime.UtcNow;

            await _instanceRepository.UpdateAsync(instance);

            _logger.LogWarning("ApiKey régénérée pour {Id}", id);

            return ApiResponse<string>.SuccessResponse(newApiKey, "Nouvelle ApiKey");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur regenerate key {Id}", id);
            return ApiResponse<string>.Failure("Erreur interne");
        }
    }
}