using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Application.Interfaces.IOtpService;
using MyApp.Domain.Entities;
using Serilog;

namespace MyApp.Application.Services.User;

public class UserService
{
    private readonly IGenericRepository<ApplicationUser> _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(
        IGenericRepository<ApplicationUser> userRepository,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ApplicationUser?> GetUserById(string id)
    {
        try
        {
            Log.Information("Retrieving user by ID {UserId}", id);
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            return user;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while retrieving user {UserId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
    {
        try
        {
            Log.Information("Retrieving all users");
            return await _userRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving users");
            throw;
        }
    }

    public async Task<ApplicationUser> AddUser(CreateUserDto dto)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            // ✅ Role assignment
            var role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role;

            if (!await _roleManager.RoleExistsAsync(role))
                throw new Exception($"Role '{role}' does not exist");

            await _userManager.AddToRoleAsync(user, role);

            Log.Information("User {Email} created with role {Role}", dto.Email, role);
            return user;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating user {Email}", dto.Email);
            throw;
        }
    }

    public async Task<ApplicationUser> UpdateUser(string id, UpdateUserDto dto)
    {
        try
        {
            Log.Information("Updating user {UserId}", id);
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UserName = dto.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            Log.Information("User {UserId} updated successfully", id);
            return user;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while updating user {UserId}", id);
            throw;
        }
    }

    public async Task DeleteUser(string id)
    {
        try
        {
            Log.Information("Deleting user {UserId}", id);
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            Log.Information("User {UserId} deleted successfully", id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while deleting user {UserId}", id);
            throw;
        }
    }




    public async Task<ApplicationUser> ChangePasswordWithOtpAsync(ChangePasswordDto dto, IOtpService otpService)
    {
        

        // Vérifier que le mot de passe et la confirmation correspondent
        if (dto.NewPassword != dto.ConfirmPassword)
            throw new Exception("Les mots de passe ne correspondent pas");

        // Récupérer l'utilisateur
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new Exception("Utilisateur introuvable");

        // Générer un token de réinitialisation du mot de passe
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return user;
    }


    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Utilisateur non trouvé");
            }

            var roles = await _userManager.GetRolesAsync(user);
            Log.Information("Rôles récupérés pour l'utilisateur {UserId}: {Roles}", userId, string.Join(", ", roles));

            return roles;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erreur lors de la récupération des rôles de l'utilisateur {UserId}", userId);
            throw;
        }
    }
}
